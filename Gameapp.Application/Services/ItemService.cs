using AutoMapper;
using Gameapp.Application.Dtos;
using Gameapp.Application.Interfaces.Repositories;
using Gameapp.Application.Interfaces.Services;
using Gameapp.Application.Models;

namespace Gameapp.Application.Services;

public class ItemService : IItemService
{
    private readonly IMapper _mapper;
    private readonly IItemRepository _itemRepository;

    public ItemService(IMapper mapper, IItemRepository itemRepository)
    {
        _mapper = mapper;
        _itemRepository = itemRepository;
    }

    public async Task<ItemDto?> Get(int id)
    {
        Item? item = await _itemRepository.Get(id);

        if (item == null)
        {
            return null;
        }

        return _mapper.Map<ItemDto>(item);
    }

    public async Task<IEnumerable<ItemDto>?> GetAll()
    {
        IEnumerable<Item> itemList = await _itemRepository.GetAll();

        if (itemList == null)
        {
            return null;
        }

        return _mapper.Map<IEnumerable<Item>, IEnumerable<ItemDto>>(itemList);
    }

    public async Task<ItemDto> Add(ItemDto itemDto)
    {
        Item item = _mapper.Map<Item>(itemDto);
        Item newItem = await _itemRepository.Add(item);

        return _mapper.Map<ItemDto>(newItem);
    }

    public async Task<ItemDto?> Update(int id, ItemDto itemDto)
    {
        if (_itemRepository.ItemExists(id))
        {
            Item item = _mapper.Map<Item>(itemDto);
            item.Id = id;

            Item? newItem = await _itemRepository.Update(item);
            if (newItem == null)
            {
                return null;
            }

            return _mapper.Map<ItemDto>(newItem);
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> Delete(int id)
    {
        if (_itemRepository.ItemExists(id))
        {
            bool success = await _itemRepository.Delete(id);

            return success;
        }
        else
        {
            return false;
        }
    }
}
