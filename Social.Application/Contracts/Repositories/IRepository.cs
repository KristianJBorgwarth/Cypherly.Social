using Social.Domain.Abstractions;

namespace Social.Application.Contracts.Repositories;

public interface IRepository<T> where T : AggregateRoot
{
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
}

