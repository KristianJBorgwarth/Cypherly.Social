using Cypherly.UserManagement.Domain.Abstractions;

namespace Social.Application.Contracts.Repositories;

public interface IRepository<T> where T : AggregateRoot
{
    Task CreateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetByIdAsync(Guid id);
    Task UpdateAsync(T entity);
}