using Cypherly.Message.Contracts.Messages.Device;
using Cypherly.Message.Contracts.Responses.Device;
using MassTransit;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;

namespace Social.Application.Features.UserProfile.Consumers;

public sealed class ConnectionIdsProxyConsumer(
        IUserProfileRepository userProfileRepository,
        IConnectionIdProvider connectionIdProvider,
        ILogger<ConnectionIdsProxyConsumer> logger)
        : IConsumer<ConnectionIdsProxyMessage>
{
    public async Task Consume(ConsumeContext<ConnectionIdsProxyMessage> ctx)
    {
        try
        {
            var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendshipsSpec(ctx.Message.TenantId), ctx.CancellationToken);
            if (userProfile is null)
            {
                logger.LogWarning("UserProfile with ID {UserId} not found", ctx.Message.TenantId);
                throw new Exception($"UserProfile with ID {ctx.Message.TenantId} not found");
            }

            var friends = userProfile.GetFriends();

            var response = await connectionIdProvider.GetConnectionIdsMultipleTenants([.. friends.Select(f => f.Id), userProfile.Id], ctx.CancellationToken);

            await ctx.RespondAsync(new ConnectionIdsProxyResponse 
            {
                CorrelationId = ctx.Message.CorrelationId,
                CausationId = ctx.Message.Id,
                ConnectionIds = response,
                UserTag = userProfile.UserTag.Tag
            }); 

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing ConnectionIdsProxyMessage for TenantId: {TenantId}", string.Join(", ", ctx.Message.TenantId));
            throw;
        }
    }
}
