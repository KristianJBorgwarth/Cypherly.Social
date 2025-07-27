namespace Cypherly.UserManagement.Application.Contracts.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}