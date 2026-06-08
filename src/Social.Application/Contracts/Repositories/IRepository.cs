using Social.Application.Abstractions;
using Social.Domain.Abstractions;

namespace Social.Application.Contracts.Repositories;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetSingleAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task CreateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
}

