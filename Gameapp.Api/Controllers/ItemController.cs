using Gameapp.Application.Dtos;
using Gameapp.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gameapp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> Get(int id)
    {
        ItemDto? itemDto = await _itemService.Get(id);

        if (itemDto == null)
        {
            return NotFound();
        }

        return Ok(itemDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetAll()
    {
        IEnumerable<ItemDto>? itemList = await _itemService.GetAll();

        return Ok(itemList);
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> Add(ItemDto itemDto)
    {
        ItemDto newItem = await _itemService.Add(itemDto);

        return CreatedAtAction(nameof(Get), new { id = newItem.Id }, newItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ItemDto itemDto)
    {
        if (id != itemDto.Id)
        {
            return BadRequest();
        }

        ItemDto? newItem = await _itemService.Update(id, itemDto);

        if (newItem == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool success = await _itemService.Delete(id);

        if (success)
        {
            return NoContent();
        }
        else
        {
            return BadRequest();
        }
    }
}
