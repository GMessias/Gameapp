using Gameapp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Gameapp.Infrastructure.Data;

public class GameContext : DbContext
{
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
    }

    public DbSet<Item> Items { get; set; }
}
