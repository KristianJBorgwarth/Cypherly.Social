using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Commands.Update.Avatar;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Infrastructure.Persistence.Context;
using Social.Test.Integration.Setup;
using Social.Test.Integration.Setup.Helpers;
using Xunit;

namespace Social.Test.Integration.UserProfileTest.CommandTest.UpdateTest.Avatar;

public class UpdateAvatarCommandHandlerTest : IntegrationTestBase
{
    private readonly UpdateAvatarCommandHandler _sut;
    private readonly IAvatarService avatarService;
    public UpdateAvatarCommandHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        avatarService = scope.ServiceProvider.GetRequiredService<IAvatarService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        _sut = new(repo, unitOfWork, avatarService);
    }

    [Fact]
    public async Task Handle_ValidUpdateCommand_Should_Update_ProfilePic_And_Return_Dto()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var command = new UpdateAvatarCommand()
        {
            TenantId = user.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "test/Social.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png")
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.RequiredValue.Etag.Should().NotBeNullOrEmpty();
        result.RequiredValue.FileKey.Should().NotBeEmpty();
        var updatedUser = await Db.UserProfile.AsNoTracking().Include(userProfile => userProfile.Avatar).FirstOrDefaultAsync(x => x.Id == user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser.Avatar.Should().NotBeNull();
        var avatarStream = avatarService.Get(result.RequiredValue.FileKey);
        avatarStream.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ValidUpdateCommand_GivenExistingAvatar_ShouldOverride_WhenUpdating()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var setupCommand = new UpdateAvatarCommand()
        {
            TenantId = user.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "test/Social.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png")
        };
        var setupResult = await _sut.Handle(setupCommand, CancellationToken.None);
        var userProfile = await Db.UserProfile.AsNoTracking().Include(up => up.Avatar).FirstOrDefaultAsync(up => up.Id == user.Id);
        userProfile.Should().NotBeNull();
        userProfile.Avatar.Should().NotBeNull();

        var existingAvatarStream = avatarService.Get(userProfile.Avatar.FileKey);

        var updateCommand = new UpdateAvatarCommand()
        {
            TenantId = user.Id,
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "test/Social.Test.Integration/Setup/Resources/cm-purple.jpg"), "image/jpg")
        };

        // Act
        var result = await _sut.Handle(updateCommand, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.RequiredValue.Etag.Should().NotBeNullOrEmpty();
        result.RequiredValue.FileKey.Should().NotBeEmpty();
        result.RequiredValue.FileKey.Should().Be(setupResult.RequiredValue.FileKey); 
        var newAvatarStream = avatarService.Get(result.RequiredValue.FileKey);
        newAvatarStream.Should().NotBeNull();
        existingAvatarStream.Should().NotHaveLength(newAvatarStream.Length);

    }

    [Fact]
    public async Task Handle_Invalid_Command_WrongID_Should_Return_NotFound_Fail_Result()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();

        var command = new UpdateAvatarCommand()
        {
            TenantId = Guid.NewGuid(),
            NewProfilePicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "test/Social.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png")
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error!.Description.Should().Be($"Could not find UserProfile with ID {command.TenantId}.");
    }
}
