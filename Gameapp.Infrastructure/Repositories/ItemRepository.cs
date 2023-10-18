using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using Gameapp.Infrastructure.Data;

namespace Gameapp.Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly GameContext _context;

    public ItemRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Item> CreateAsync(Item item)
    {
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task UpdateAsync(Item item)
    {
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Item item)
    {
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
    }
}
