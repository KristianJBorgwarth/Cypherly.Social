using Social.Domain.Aggregates;

namespace Social.Application.Contracts.Repositories;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByUserTag(string userTag);
}