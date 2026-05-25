using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.Dtos;

namespace Social.Domain.Interfaces;

public interface IFriendshipService
{
    Result CreateFriendship(UserProfile userProfile, UserProfile friendProfile);
    Result AcceptFriendship(UserProfile userProfile, string friendTag);
    Result DeleteFriendship(UserProfile userProfile, string friendTag);
    Result DeleteFriendRequest(UserProfile userProfile, string friendTag);
    FriendshipStatusDto GetFriendshipStatus(UserProfile userProfile, string friendTag);
    void MarkeFriendshipAsSeen(UserProfile userProfile, string[] tags);
}