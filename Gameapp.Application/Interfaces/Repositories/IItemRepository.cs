using Gameapp.Application.Models;

namespace Gameapp.Application.Interfaces.Repositories;

public interface IItemRepository
{
    Task<Item?> Get(int id);
    Task<IEnumerable<Item>> GetAll();
    Task<Item> Add(Item item);
    Task<Item?> Update(Item item);
    Task<bool> Delete(int id);
    bool ItemExists(int id);
}
