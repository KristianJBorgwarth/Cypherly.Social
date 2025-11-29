using Social.Application.Features.UserProfile.Commands.Create.Friendship;
using Social.Application.Features.UserProfile.Commands.Delete.Friendship;
using Social.Application.Features.UserProfile.Commands.Update.AcceptFriendship;
using Social.Application.Features.UserProfile.Commands.Update.BlockUser;
using Social.Application.Features.UserProfile.Commands.Update.DisplayName;
using Social.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen;
using Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;
using Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;
using Social.Application.Features.UserProfile.Commands.Update.UnblockUser;
using Social.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;
using Social.Application.Features.UserProfile.Queries.GetFriendRequests;
using Social.Application.Features.UserProfile.Queries.GetFriends;
using Social.Application.Features.UserProfile.Queries.GetUserProfile;
using Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using Social.Application.Features.UserProfile.Queries.GetUserProfilePicture;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.API.Common;
using Social.API.Requests.Command;
using Social.API.Requests.Query;

namespace Social.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UserProfileController(ISender sender) : BaseController
{
    [HttpGet("")]
    [ProducesResponseType(typeof(GetUserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserProfile(CancellationToken cancellationToken = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetUserProfileQuery { TenantId = tenantId }, cancellationToken);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpGet("tag")]
    [ProducesResponseType(typeof(GetUserProfileByTagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetUserProfileByTag([FromQuery] GetUserProfileByTagRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetUserProfileByTagQuery { TenantId = tenantId, Tag = request.Tag },
            cancellationToken);
        if (result.Success is false) return Error(result.Error);

        return result.Value is not null ? Ok(result.Value) : NoContent();
    }

    [HttpGet("profile-picture")]
    [ProducesResponseType(typeof(GetUserProfilePictureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfilePicture([FromQuery] GetUserProfilePictureRequest request,
        CancellationToken ct = default)
    {
        var query = new GetUserProfilePictureQuery { ProfilePictureUrl = request.ProfilePictureUrl };
        var result = await sender.Send(query, ct);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPut("profile-picture")]
    [ProducesResponseType(typeof(UpdateUserProfilePictureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateUserProfilePictureRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new UpdateUserProfilePictureCommand
            { TenantId = tenantId, NewProfilePicture = request.ProfilePicture }, ct);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPut("displayname")]
    [ProducesResponseType(typeof(UpdateUserProfileDisplayNameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDisplayName([FromBody] UpdateDisplayNameRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result =
            await sender.Send(
                new UpdateUserProfileDisplayNameCommand { TenantId = tenantId, DisplayName = request.DisplayName }, ct);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPut("block-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result =
            await sender.Send(new BlockUserCommand() { TenantId = tenantId, BlockedUserTag = request.BlockedUserTag },
                ct);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpPut("unblock-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnblockUser([FromBody] UnblockUserRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new UnblockUserCommand { TenantId = tenantId, Tag = request.Tag }, ct);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpGet("blocked-users")]
    [ProducesResponseType(typeof(GetBlockedUserProfilesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBlockedUserProfiles(CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetBlockedUserProfilesQuery { TenantId = tenantId }, ct);
        if (result.Success is false) return Error(result.Error);

        return result.Value!.Count > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpPost("friendship")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateFriendship([FromBody] CreateFriendshipRequest request,
        CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new CreateFriendshipCommand
            { TenantId = tenantId, FriendTag = request.FriendTag }, ct);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpGet("friendships")]
    [ProducesResponseType(typeof(GetFriendsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriends(CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetFriendsQuery {TenantId = tenantId}, ct);

        if (result.Success is false) return Error(result.Error);

        return result.Value!.Count > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpGet("friendship/requests")]
    [ProducesResponseType(typeof(GetFriendRequestsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriendRequests(CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetFriendRequestsQuery {TenantId = tenantId}, ct);

        if (result.Success is false) return Error(result.Error);

        return result.Value!.Length > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpPut("friendship/accept")]
    [ProducesResponseType(typeof(AcceptFriendshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptFriendship([FromBody] AcceptFriendshipRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new AcceptFriendshipCommand {TenantId = tenantId, FriendTag = request.FriendTag}, ct);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpDelete("friendship/delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFriendship([FromQuery] DeleteFriendshipRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new DeleteFriendshipCommand {TenantId = tenantId, FriendTag = request.FriendTag}, ct);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpPut("friendship/seen")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkFriendRequestsAsSeen([FromBody] MarkFriendRequestsAsSeenRequest friendRequest, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new MarkFriendRequestsAsSeenCommand {TenantId = tenantId, RequestTags = friendRequest.RequestTags}, ct);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpPut("toggle-privacy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TogglePrivacy([FromBody] TogglePrivacyRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new TogglePrivacyCommand {TenantId = tenantId, IsPrivate = request.IsPrivate}, ct);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpDelete("friendship/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectFriendship([FromQuery] DeleteFriendRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new DeleteFriendshipCommand {TenantId = tenantId, FriendTag = request.FriendTag}, ct);
        return result.Success ? Ok() : Error(result.Error);
    }
}
