using Gameapp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gameapp.Application.Contracts;

public interface IApplicationDbContext
{
    DbSet<Item> Items { get; set; }
}
