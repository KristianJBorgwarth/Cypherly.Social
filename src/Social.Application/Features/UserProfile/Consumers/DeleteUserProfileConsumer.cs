using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Enums;
using Cypherly.Message.Contracts.Messages.Common;
using Cypherly.Message.Contracts.Messages.User;
using Social.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;

namespace Social.Application.Features.UserProfile.Consumers;

public class DeleteUserProfileConsumer(
    IUserProfileRepository userProfileRepository,
    IUserProfileLifecycleService userProfileLifecycleService,
    IUnitOfWork unitOfWork,
    IProducer<OperationSucceededMessage> producer,
    ILogger<DeleteUserProfileConsumer> logger)
    : IConsumer<UserDeleteMessage>
{
    public async Task Consume(ConsumeContext<UserDeleteMessage> ctx)
    {
        try
        {
            var message = ctx.Message;
            var user = await userProfileRepository.GetSingleAsync(new UserProfileSpec(message.UserProfileId));

            if (user is null)
            {
                logger.LogError("User with id {UserProfileId} not found", message.UserProfileId);
                throw new KeyNotFoundException($"User with id {message.UserProfileId} not found.");
            }

            userProfileLifecycleService.SoftDelete(user);
            await unitOfWork.SaveChangesAsync();

            var responseMessage = new OperationSucceededMessage()
            {
                CorrelationId = ctx.Message.CorrelationId,
                CausationId = ctx.Message.Id,
                OperationType = OperationType.UserProfileDelete,
            };
            
            await producer.PublishMessageAsync(responseMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An Exception occured while attempting to delete a user profile");
            throw;
        }
    }
}
