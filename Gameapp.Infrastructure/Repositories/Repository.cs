using Gameapp.Domain.Repositories;
using Gameapp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Gameapp.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly GameContext _gameContext;
    protected DbSet<T> _entities;

    public Repository(GameContext gameContext)
    {
        _gameContext = gameContext;
        _entities = _gameContext.Set<T>();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _entities.AddAsync(entity);

        return entity;
    }

    public void Delete(T entity)
    {
        _entities.Remove(entity);
    }

    public void Update(T entity)
    {
        _entities.Update(entity);
    }
}
