using Social.Application.Features.UserProfile.Commands.Create.Friendship;
using Social.Application.Features.UserProfile.Commands.Delete.FriendRequest;
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
using Social.API.Filters;

namespace Social.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UserProfileController(ISender sender) : BaseController
{

    [HttpGet("")]
    [ProducesResponseType(typeof(GetUserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserProfile([FromQuery] GetUserProfileQuery query)
    {
        var result = await sender.Send(query);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpGet("tag")]
    [ProducesResponseType(typeof(GetUserProfileByTagDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetUserProfileByTag([FromQuery] GetUserProfileByTagQuery query)
    {
        var result = await sender.Send(query);
        if (result.Success is false) return Error(result.Error);

        return result.Value is not null ? Ok(result.Value) : NoContent();
    }

    [HttpGet("profile-picture")]
    [ProducesResponseType(typeof(GetUserProfilePictureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfilePicture([FromQuery] GetUserProfilePictureQuery query)
    {
        var result = await sender.Send(query);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPut("profile-picture")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(typeof(UpdateUserProfilePictureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfilePicture([FromForm] UpdateUserProfilePictureCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPut("displayname")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(typeof(UpdateUserProfileDisplayNameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDisplayName([FromBody] UpdateUserProfileDisplayNameCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPut("block-user")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpPut("unblock-user")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnblockUser([FromBody] UnblockUserCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpGet("blocked-users")]
    [ProducesResponseType(typeof(GetBlockedUserProfilesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBlockedUserProfiles([FromQuery] GetBlockedUserProfilesQuery query)
    {
        var result = await sender.Send(query);
        if (result.Success is false) return Error(result.Error);

        return result.Value!.Count > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpPost("friendship/create")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateFriendship([FromBody] CreateFriendshipCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpGet("friendships")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(typeof(GetFriendsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriends([FromQuery] GetFriendsQuery query)
    {
        var result = await sender.Send(query);

        if (result.Success is false) return Error(result.Error);

        return result.Value!.Count > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpGet("friendship/requests")]
    [ProducesResponseType(typeof(GetFriendRequestsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriendRequests([FromQuery] GetFriendRequestsQuery query)
    {
        var result = await sender.Send(query);

        if (result.Success is false) return Error(result.Error);

        return result.Value!.Length > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpPut("friendship/accept")]
    [ProducesResponseType(typeof(AcceptFriendshipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptFriendship([FromBody] AcceptFriendshipCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpDelete("friendship/delete")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFriendship([FromQuery] DeleteFriendshipCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpPut("friendship/mark-seen")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkFriendRequestsAsSeen([FromBody] MarkFriendRequestsReadCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpPut("toggle-privacy")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TogglePrivacy([FromBody] TogglePrivacyCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpDelete("friendship/reject")]
    [ServiceFilter(typeof(IValidateUserIdFilter))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RejectFriendship([FromQuery] DeleteFriendRequestCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }
}