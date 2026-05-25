using Social.Application.Features.UserProfile.Commands.Update.DisplayName;
using Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;
using Social.Application.Features.UserProfile.Queries.GetUserProfile;
using Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.API.Common;
using Social.API.Requests.Command;
using Social.API.Requests.Query;
using Social.Application.Features.Friendships.Commands.Create;
using Social.Application.Features.Friendships.Commands.Delete.Friendship;
using Social.Application.Features.Friendships.Commands.Update.AcceptFriendship;
using Social.Application.Features.Friendships.Commands.Update.BlockUser;
using Social.Application.Features.Friendships.Commands.Update.MarkFriendRequestAsSeen;
using Social.Application.Features.Friendships.Commands.Update.UnblockUser;
using Social.Application.Features.Friendships.Queries.GetBlockedUserProfiles;
using Social.Application.Features.Friendships.Queries.GetFriendRequests;
using Social.Application.Features.Friendships.Queries.GetFriends;
using Social.Application.Features.UserProfile.Queries.GetAvatar;
using Social.Application.Features.UserProfile.Commands.Update.Avatar;

namespace Social.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserProfileController(ISender sender) : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType(typeof(GetUserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserProfile(CancellationToken cancellationToken = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetUserProfileQuery { TenantId = tenantId }, cancellationToken);
        return result.Success ? Ok(result.Value) : result.ToProblemDetails();
    }

    [HttpGet("tag")]
    [ProducesResponseType(typeof(GetUserProfileByTagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetUserProfileByTag([FromQuery] GetUserProfileByTagRequest req, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetUserProfileByTagQuery { TenantId = tenantId, Tag = req.Tag },
            ct);
        if (!result.Success) return result.ToProblemDetails();

        return result.Value is not null ? Ok(result.Value) : NoContent();
    }

    [HttpGet("avatar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> GetAvatar([FromQuery] GetAvatarRequest req, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAvatarQuery { FileKey = req.FileKey, ETag = Request.GetETag() }, ct);
        if (!result.Success) return result.ToProblemDetails();

        if (result.RequiredValue.ETag is not null)
            Response.Headers.ETag = result.RequiredValue.ETag;

        return result.RequiredValue.IsModified
            ? File(result.RequiredValue.Content!, result.RequiredValue.ContentType!)
            : StatusCode(StatusCodes.Status304NotModified);
    }

    [HttpPut("avatar")]
    [ProducesResponseType(typeof(UpdateAvatarDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateAvatarRequest req, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new UpdateAvatarCommand() 
        { TenantId = tenantId, Avatar = req.Avatar }, ct);
        return result.Success ? Ok(result.Value) : result.ToProblemDetails();
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
        return result.Success ? Ok(result.Value) : result.ToProblemDetails();
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
        return result.Success ? Ok() : result.ToProblemDetails();
    }

    [HttpPut("unblock-user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnblockUser([FromBody] UnblockUserRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new UnblockUserCommand { TenantId = tenantId, Tag = request.Tag }, ct);
        return result.Success ? Ok() : result.ToProblemDetails();
    }

    [HttpGet("blocked-users")]
    [ProducesResponseType(typeof(GetBlockedUserProfilesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBlockedUserProfiles(CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetBlockedUserProfilesQuery { TenantId = tenantId }, ct);
        if (!result.Success) return result.ToProblemDetails();

        return result.Value!.Count > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpPost("friendship")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFriendship([FromBody] CreateFriendshipRequest request,
        CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new CreateFriendshipCommand
        { TenantId = tenantId, FriendTag = request.FriendTag }, ct);
        return result.Success ? Ok() : result.ToProblemDetails();
    }

    [HttpGet("friendships")]
    [ProducesResponseType(typeof(GetFriendsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriends(CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetFriendsQuery { TenantId = tenantId }, ct);

        if (!result.Success) return result.ToProblemDetails();

        return result.Value!.Count > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpGet("friendship/requests")]
    [ProducesResponseType(typeof(GetFriendRequestsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriendRequests(CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new GetFriendRequestsQuery { TenantId = tenantId }, ct);

        if (!result.Success) return result.ToProblemDetails();

        return result.Value!.Length > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpPut("friendship/accept")]
    [ProducesResponseType(typeof(AcceptFriendshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptFriendship([FromBody] AcceptFriendshipRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new AcceptFriendshipCommand { TenantId = tenantId, FriendTag = request.FriendTag }, ct);
        return result.Success ? Ok(result.Value) : result.ToProblemDetails();
    }

    [HttpDelete("friendship/delete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFriendship([FromQuery] DeleteFriendshipRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new DeleteFriendshipCommand { TenantId = tenantId, FriendTag = request.FriendTag }, ct);
        return result.Success ? Ok() : result.ToProblemDetails();
    }

    [HttpPut("friendship/seen")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkFriendRequestsAsSeen([FromBody] MarkFriendRequestsAsSeenRequest friendRequest, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new MarkFriendRequestsAsSeenCommand { TenantId = tenantId, RequestTags = friendRequest.RequestTags }, ct);
        return result.Success ? Ok() : result.ToProblemDetails();
    }

    [HttpPut("toggle-privacy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TogglePrivacy([FromBody] TogglePrivacyRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new TogglePrivacyCommand { TenantId = tenantId, IsPrivate = request.IsPrivate }, ct);
        return result.Success ? Ok() : result.ToProblemDetails();
    }

    [HttpDelete("friendship/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectFriendship([FromQuery] DeleteFriendRequest request, CancellationToken ct = default)
    {
        var tenantId = User.GetUserId();
        var result = await sender.Send(new DeleteFriendshipCommand { TenantId = tenantId, FriendTag = request.FriendTag }, ct);
        return result.Success ? Ok() : result.ToProblemDetails();
    }
}
