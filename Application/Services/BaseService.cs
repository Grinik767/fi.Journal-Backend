using Application.Extensions;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Exceptions;
using Infrastructure.Repositories;

namespace Application.Services;

public class BaseService<T>(IRepository<T> repository, IValidator<T> validator)
    where T : Entity<Guid>
{
    public async Task Delete(Guid id, CancellationToken ct) => await repository.Delete(id, ct);

    public async Task<List<T>> GetAll(CancellationToken ct) => await repository.GetAll(ct);

    public async Task<T> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            return await repository.GetById(id, ct);
        }
        catch (InvalidOperationException e)
        {
            throw new EntityNotFoundException("Object not found", e);
        }
    }
    
    protected virtual async Task<T> Add(T entity, CancellationToken ct)
    {
        await entity.ValidateAsync(validator, ct);
        await repository.Add(entity, ct);

        return entity;
    }

    protected async Task<T> Update(T entity, CancellationToken ct)
    {
        await entity.ValidateAsync(validator, ct);
        return await repository.Update(entity, ct);
    }
}