using Gameapp.Application.Features.Items.Commands.CreateItem;
using Gameapp.Application.Features.Items.Commands.DeleteItem;
using Gameapp.Application.Features.Items.Commands.UpdateItem;
using Gameapp.Application.Features.Items.Queries.GetAllItems;
using Gameapp.Application.Features.Items.Queries.GetItemById;
using Gameapp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gameapp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Item>> GetById(Guid id)
    {
        Item? item = await _mediator.Send(new GetItemByIdQuery { Id = id });

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Item>>> GetAll()
    {
        IEnumerable<Item>? itemList = await _mediator.Send(new GetAllItemsQuery());

        return Ok(itemList);
    }

    [HttpPost]
    public async Task<ActionResult<Item>> Create(CreateItemCommand command)
    {
        Item item = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteItemCommand { Id = id });

        return NoContent();
    }
}
