namespace Gameapp.Domain.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> CreateAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
