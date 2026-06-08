using Social.Application.Abstractions;
using Social.Domain.Abstractions;

namespace Social.Infrastructure.Persistence.Repositories;

internal sealed class ReadRepository<T> : IReadRepository<T> where T : Entity
{
    public Task<IReadOnlyList<T>> GetListAsync(ISpecification<T> spec, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetSingleAsync(ISpecification<T> spec, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
