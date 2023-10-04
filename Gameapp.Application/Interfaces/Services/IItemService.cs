using Gameapp.Application.Dtos;

namespace Gameapp.Application.Interfaces.Services;

public interface IItemService
{
    Task<ItemDto?> Get(int id);
    Task<IEnumerable<ItemDto>?> GetAll();
    Task<ItemDto> Add(ItemDto itemDto);
    Task<ItemDto?> Update(int id, ItemDto itemDto);
    Task<bool> Delete(int id);
}
