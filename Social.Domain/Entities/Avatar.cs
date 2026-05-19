using Social.Domain.Abstractions;
using Social.Domain.ValueObjects;

namespace Social.Domain.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public sealed class Avatar : Entity
{
    public Guid UserProfileId { get; private set; }
    public ETag Etag { get; private set; }

    public Avatar() { } // For EF Core

    public Avatar(Guid userProfileId, Guid avatarId, ETag etag) : base(avatarId)
    {
        UserProfileId = userProfileId;
        Etag = etag;
    }
}
