using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Commands.Update.Avatar;
using Social.Domain.Aggregates;
using Social.Domain.Common;

public class UpdateAvatarCommandHandler(
   IUserProfileRepository upRepo,
   IUnitOfWork uow,
   IAvatarService ats)
    : ICommandHandler<UpdateAvatarCommand, UpdateAvatarDto>
{
    public async Task<Result<UpdateAvatarDto>> Handle(UpdateAvatarCommand cmd, CancellationToken ct)
    {
        var userProfile = await upRepo.GetSingleAsync(new UserProfileWithAvatarSpec(cmd.TenantId), ct);
        if (userProfile is null)
            return Result.Fail<UpdateAvatarDto>(Error.NotFound<UserProfile>(cmd.TenantId.ToString()));

        var avatar = userProfile.GetOrCreateAvatar(cmd.Avatar.ContentType);

        var uploadResult = await ats.UploadAsync(cmd.Avatar, avatar.FileKey, ct);
        if (!uploadResult.Success)
            return Result.Fail<UpdateAvatarDto>(uploadResult.Error);

        await uow.SaveChangesAsync(ct);

        return Result.Ok(new UpdateAvatarDto()
        {
            FileKey = avatar.FileKey,
            Etag = avatar.Etag.Value
        });
    }
}
