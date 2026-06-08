using Social.Application.Abstractions;
using Social.Domain.Abstractions;

public interface IReadRepository<T> where T : Entity
{
    Task<T?> GetSingleAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetListAsync(ISpecification<T> spec, CancellationToken ct = default);
}
