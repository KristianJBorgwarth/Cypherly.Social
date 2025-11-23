using Cypherly.Message.Contracts.Messages.Device;
using Cypherly.Message.Contracts.Responses.Device;
using MassTransit;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Consumers;

public sealed class ConnectionIdsProxyConsumer(
        IUserProfileRepository userProfileRepository,
        IConnectionIdProvider connectionIdProvider,
        ILogger<ConnectionIdsProxyConsumer> logger)
        : IConsumer<ConnectionIdsProxyMessage>
{
    public async Task Consume(ConsumeContext<ConnectionIdsProxyMessage> context)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(context.Message.TenantId);
            if (userProfile is null)
            {
                logger.LogWarning("UserProfile with ID {UserId} not found", context.Message.TenantId);
                throw new Exception($"UserProfile with ID {context.Message.TenantId} not found");
            }

            var friends = userProfile.GetFriends();

            var response = await connectionIdProvider.GetConnectionIdsMultipleTenants([.. friends.Select(f => f.Id), userProfile.Id], context.CancellationToken);

            await context.RespondAsync(new ConnectionIdsProxyResponse 
            {
                CorrelationId = context.Message.CorrelationId,
                CausationId = context.Message.Id,
                ConnectionIds = response,
                UserTag = userProfile.UserTag.Tag
            }); 

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing ConnectionIdsProxyMessage for TenantId: {TenantId}", string.Join(", ", context.Message.TenantId));
            throw;
        }
    }
}
