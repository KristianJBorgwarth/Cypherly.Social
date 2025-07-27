using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Enums;
using Cypherly.Message.Contracts.Messages.Common;
using Cypherly.Message.Contracts.Messages.User;
using Social.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Consumers;

public class DeleteUserProfileConsumer(
    IUserProfileRepository userProfileRepository,
    IUserProfileLifecycleService userProfileLifecycleService,
    IUnitOfWork unitOfWork,
    IProducer<OperationSucceededMessage> producer,
    ILogger<DeleteUserProfileConsumer> logger)
    : IConsumer<UserDeleteMessage>
{
    public async Task Consume(ConsumeContext<UserDeleteMessage> context)
    {
        try
        {
            var message = context.Message;
            var user = await userProfileRepository.GetByIdAsync(message.UserProfileId);

            if (user is null)
            {
                logger.LogError("User with id {UserProfileId} not found", message.UserProfileId);
                throw new KeyNotFoundException($"User with id {message.UserProfileId} not found.");
            }

            userProfileLifecycleService.SoftDelete(user);
            await unitOfWork.SaveChangesAsync();

            var responseMessage = new OperationSucceededMessage()
            {
                CorrelationId = context.Message.CorrelationId,
                CausationId = context.Message.Id,
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