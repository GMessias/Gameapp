using Gameapp.Domain.Entities;

namespace Gameapp.Domain.Repositories;

public interface IItemRepository
{
    Task<Item> CreateAsync(Item item);
    Task UpdateAsync(Item item);
    Task DeleteAsync(Item item);
}
