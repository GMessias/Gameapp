using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Gameapp.Infrastructure.Data;

public class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
{
    public GameContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
        
        string currentDirectory = Directory.GetCurrentDirectory();
        string parentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? string.Empty;
        string basePath = args.Length > 0 ? Path.Combine(parentDirectory, args[0]) : currentDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        string? connectionString = configuration.GetConnectionString("GameDatabase");
        optionsBuilder.UseNpgsql(connectionString);

        return new GameContext(optionsBuilder.Options);
    }
}
