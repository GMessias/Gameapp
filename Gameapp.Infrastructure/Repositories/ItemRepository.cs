using Gameapp.Application.Interfaces.Repositories;
using Gameapp.Application.Models;
using Gameapp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gameapp.Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly GameContext _context;

    public ItemRepository(GameContext context)
    {
        _context = context;
    }

    public async Task<Item?> Get(int id)
    {
        return await _context.Items.FindAsync(id);
    }

    public async Task<IEnumerable<Item>> GetAll()
    {
        return await _context.Items.ToListAsync();
    }

    public async Task<Item> Add(Item item)
    {
        item.CreatedAt = DateTime.Now;

        _context.Add(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<Item?> Update(Item item)
    {
        Item? oldItem = await Get(item.Id);

        if (oldItem == null)
        {
            return null;
        }

        oldItem.Name = item.Name;
        oldItem.Description = item.Description;
        oldItem.ItemType = item.ItemType;
        oldItem.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return oldItem;
    }

    public async Task<bool> Delete(int id)
    {
        Item? item = await Get(id);

        if (item == null)
        {
            return false;
        }

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();

        return true;
    }

    public bool ItemExists(int id)
    {
        return _context.Items.Any(e => e.Id == id);
    }
}
