using Gameapp.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Gameapp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddAutoMapper(typeof(AutoMapperProfile));

        return services;
    }
}
