using Gameapp.Application.Contracts;
using Gameapp.Domain.Repositories;
using Gameapp.Infrastructure.Data;
using Gameapp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gameapp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("GameDatabase");

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddDbContext<GameContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IApplicationDbContext, GameContext>();

        return services;
    }
}
