using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Common;
using Cypherly.UserManagement.Domain.Dtos;

namespace Cypherly.UserManagement.Domain.Interfaces;

public interface IFriendshipService
{
    Result CreateFriendship(UserProfile userProfile, UserProfile friendProfile);
    Result AcceptFriendship(UserProfile userProfile, string friendTag);
    Result DeleteFriendship(UserProfile userProfile, string friendTag);
    Result DeleteFriendRequest(UserProfile userProfile, string friendTag);
    FriendshipStatusDto GetFriendshipStatus(UserProfile userProfile, string friendTag);
    void MarkeFriendshipAsSeen(UserProfile userProfile, string[] tags);
}