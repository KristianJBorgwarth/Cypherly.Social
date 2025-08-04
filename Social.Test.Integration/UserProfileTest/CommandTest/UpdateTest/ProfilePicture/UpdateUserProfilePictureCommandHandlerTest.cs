using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;
using Social.Test.Integration.Setup.Helpers;

namespace Social.Test.Integration.UserProfileTest.CommandTest.UpdateTest.ProfilePicture;

public class UpdateUserProfilePictureCommandHandlerTest : IntegrationTestBase
{
    private readonly UpdateUserProfilePictureCommandHandler _sut;
    public UpdateUserProfilePictureCommandHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UpdateUserProfilePictureCommandHandler>>();

        _sut = new(repo, profilePictureService, unitOfWork, logger);
    }

    [Fact]
    public async Task Handle_ValidUpdateCommand_Should_Update_ProfilePic_And_Return_Dto()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfilePictureCommand()
        {
            TenantId = user.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Social.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png")
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var updatedChatUser = await Db.UserProfile.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
        updatedChatUser!.ProfilePictureUrl.Should().NotBeNull();
        Db.OutboxMessage.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Invalid_Command_WrongID_Should_Return_NotFound_Fail_Result()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfilePictureCommand()
        {
            TenantId = Guid.NewGuid(),
            NewProfilePicture = null
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.NotFound(command.TenantId).Message);
    }

    [Fact]
    public async Task Handle_Invalid_Command_WrongProfilePicture_Should_Return_Fail()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(userProfile);
        await Db.SaveChangesAsync();
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfilePictureCommand()
        {
            TenantId = userProfile.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Social.Test.Integration/Setup/Resources/confirm_style_2_002.wav"), "image/jpg")
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.UnspecifiedError("Value 'Invalid file type. Only JPG, JPEG and PNG files are allowed.' is not valid in this context").Message);
    }
}