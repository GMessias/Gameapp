using Gameapp.Application.Interfaces.Services;
using Gameapp.Application.Mappings;
using Gameapp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gameapp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddScoped<IItemService, ItemService>();

        return services;
    }
}
