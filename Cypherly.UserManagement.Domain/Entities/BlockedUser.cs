using Cypherly.UserManagement.Domain.Abstractions;
using Cypherly.UserManagement.Domain.Aggregates;

namespace Cypherly.UserManagement.Domain.Entities;

public class BlockedUser : Entity
{
    public Guid BlockingUserProfileId { get; private set; }
    public Guid BlockedUserProfileId { get; private set; }

    public virtual UserProfile BlockedUserProfile { get; private set; } = null!;
    public BlockedUser() { } // EF Core

    public BlockedUser(Guid blockingUserProfileId, Guid blockedUserProfileId) : base(Guid.Empty)
    {
        BlockingUserProfileId = blockingUserProfileId;
        BlockedUserProfileId = blockedUserProfileId;
    }
}