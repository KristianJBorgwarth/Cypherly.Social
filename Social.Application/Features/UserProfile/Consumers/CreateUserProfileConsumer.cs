using Cypherly.Message.Contracts.Messages.Profile;
using Cypherly.Message.Contracts.Responses.Profile;
using Cypherly.UserManagement.Domain.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Consumers;

public class CreateUserProfileConsumer(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IUserProfileLifecycleService userProfileLifecycleService,
    ILogger<CreateUserProfileConsumer> logger)
    : IConsumer<CreateUserProfileMessage>
{
    public async Task Consume(ConsumeContext<CreateUserProfileMessage> context)
    {
        var message = context.Message;
        try
        {
            var profile = userProfileLifecycleService.CreateUserProfile(message.UserId, message.Username);

            await userProfileRepository.CreateAsync(profile);

            await unitOfWork.SaveChangesAsync();

            await context.RespondAsync(new CreateUserProfileResponse
            {
                CorrelationId = Guid.NewGuid(),
                CausationId = message.CausationId,
                IsSuccess = true,
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Exception occured while attempting to create a user profile");
            await context.RespondAsync(new CreateUserProfileResponse()
            {
                CorrelationId = Guid.NewGuid(),
                CausationId = message.CausationId,
                IsSuccess = false,
                Error = "An exception occured while creating attempting to create the user profile"
            });
            throw;
        }
    }
}