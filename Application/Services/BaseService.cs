using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;

namespace Application.Services;

public abstract class BaseService<T>(IRepository<T> repository, IValidator<T> validator)
    where T : Entity<Guid>
{
    public async Task Delete(Guid id, CancellationToken ct) => await repository.Delete(id, ct);

    public async Task<List<T>> GetAll(CancellationToken ct) => await repository.GetAll(ct);

    public async Task<T> GetById(Guid id, CancellationToken ct) => await repository.GetById(id, ct);
}