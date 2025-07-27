using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.Dtos;
using Social.Domain.Enums;
using Social.Domain.Events.UserProfile;
using Social.Domain.Interfaces;

namespace Social.Domain.Services;

public class FriendshipService : IFriendshipService
{
    public Result CreateFriendship(UserProfile userProfile, UserProfile friendProfile)
    {
        var result = userProfile.AddFriendship(friendProfile);

        if (result.Success is false)
            return Result.Fail(result.Error);

        userProfile.AddDomainEvent(new FriendshipCreatedEvent(userProfile.Id, friendProfile.Id));
        return Result.Ok();
    }

    public Result AcceptFriendship(UserProfile userProfile, string friendTag)
    {
        var friendship = userProfile.FriendshipsReceived.FirstOrDefault(f => f.UserProfile.UserTag.Tag == friendTag);

        if (friendship is null)
            return Result.Fail(Errors.General.UnspecifiedError("Friendship not found"));

        if (friendship.Status != FriendshipStatus.Pending)
            return Result.Fail(Errors.General.UnspecifiedError("Friendship not pending"));

        friendship.AcceptFriendship();
        userProfile.AddDomainEvent(new FriendshipAcceptedEvent(friendship.UserProfile.Id, userProfile.UserTag.Tag));
        return Result.Ok();
    }

    public Result DeleteFriendship(UserProfile userProfile, string friendTag)
    {
        var result = userProfile.DeleteFriendship(friendTag);

        if (result.Success is false) return Result.Fail(result.Error);

        userProfile.AddDomainEvent(new FriendshipDeletedEvent(userProfile.Id, result.Value));
        return Result.Ok();
    }

    public Result DeleteFriendRequest(UserProfile userProfile, string friendTag)
    {
        var friendship = userProfile.FriendshipsReceived.FirstOrDefault(f => f.UserProfile.UserTag.Tag == friendTag);

        if (friendship is null)
            return Result.Fail(Errors.General.UnspecifiedError("Friendship not found"));

        if (friendship.Status != FriendshipStatus.Pending)
            return Result.Fail(Errors.General.UnspecifiedError("Friendship not pending"));

        userProfile.DeleteFriendship(friendTag);

        userProfile.AddDomainEvent(new FriendRequestRejectedEvent(userProfile.Id, friendship.UserProfileId));

        return Result.Ok();
    }

    public void MarkeFriendshipAsSeen(UserProfile userProfile, string[] tags)
    {
        foreach (var tag in tags)
        {
            var friendship = userProfile.FriendshipsReceived.FirstOrDefault(f => f.UserProfile.UserTag.Tag == tag);
            friendship?.MarkAsSeen();
        }
    }

    public FriendshipStatusDto GetFriendshipStatus(UserProfile userProfile, string friendTag)
    {
        var receivedStatus = GetRecievedFriendshipStatus(userProfile, friendTag);
        if (receivedStatus is not null)
            return receivedStatus;

        var initiatedStatus = GetInitiatedFriendshipStatus(userProfile, friendTag);
        if (initiatedStatus is not null)
            return initiatedStatus;

        return new FriendshipStatusDto
        {
            Status = null,
            Direction = FriendshipDirection.None
        };
    }

    private static FriendshipStatusDto? GetRecievedFriendshipStatus(UserProfile userProfile, string friendTag)
    {
        var friendship = userProfile.FriendshipsReceived.FirstOrDefault(f => f.UserProfile.UserTag.Tag == friendTag);
        return friendship is null ? null : new FriendshipStatusDto { Status = friendship.Status, Direction = FriendshipDirection.Received };
    }

    private static FriendshipStatusDto? GetInitiatedFriendshipStatus(UserProfile userProfile, string friendTag)
    {
        var friendship = userProfile.FriendshipsInitiated.FirstOrDefault(f => f.FriendProfile.UserTag.Tag == friendTag);
        return friendship is null ? null : new FriendshipStatusDto { Status = friendship.Status, Direction = FriendshipDirection.Sent };
    }
}