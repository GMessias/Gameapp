using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using Gameapp.Infrastructure.Data;

namespace Gameapp.Infrastructure.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(GameContext context) : base(context)
    {
    }
}
