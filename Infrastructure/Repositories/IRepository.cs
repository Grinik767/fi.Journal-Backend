namespace Infrastructure.Repositories;

public interface IRepository<T>
{
    Task Add(T entity, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
    Task<T> GetById(Guid id, CancellationToken ct);
    Task<List<T>> GetAll(CancellationToken ct);
}