using Cypherly.UserManagement.Domain.Aggregates;

namespace Cypherly.UserManagement.Application.Contracts.Repositories;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByUserTag(string userTag);
}