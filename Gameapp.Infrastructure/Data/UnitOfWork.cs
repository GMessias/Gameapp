using Gameapp.Application.Contracts;

namespace Gameapp.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly GameContext _gameContext;

    public UnitOfWork(GameContext gameContext)
    {
        _gameContext = gameContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _gameContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _gameContext.Dispose();
    }
}
