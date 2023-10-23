using Gameapp.Application.Contracts;
using Gameapp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gameapp.Infrastructure.Data;

public class GameContext : DbContext, IApplicationDbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
}
