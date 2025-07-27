using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.Interfaces;
using Cypherly.UserManagement.Domain.ValueObjects;
using Cypherly.UserManagement.Infrastructure.Persistence.Context;
using Cypherly.UserManagement.Test.Integration.Setup;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Delete.FriendRequest;

namespace Cypherly.UserManagement.Test.Integration.UserProfileTest.CommandTest.DeleteTest.FriendRequest;

public class DeleteFriendRequestCommandHandlerTest : IntegrationTestBase
{
    private readonly DeleteFriendRequestCommandHandler _sut;
    public DeleteFriendRequestCommandHandlerTest(IntegrationTestFactory<Program, UserManagementDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var userProfileRepository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var friendshipService = scope.ServiceProvider.GetRequiredService<IFriendshipService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DeleteFriendRequestCommandHandler>>();
        _sut = new DeleteFriendRequestCommandHandler(
            userProfileRepository,
            unitOfWork,
            friendshipService,
            logger
        );
    }

    [Fact]
    public async Task DeleteFriendRequestCommandHandler_ShouldDeleteFriendRequest_WhenValidRequest()
    {
        // Arrange
        var recievingUser = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var requestinUser = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        requestinUser.AddFriendship(recievingUser);

        Db.UserProfile.AddRange(recievingUser, requestinUser);
        await Db.SaveChangesAsync();

        var command = new DeleteFriendRequestCommand
        {
            Id = recievingUser.Id,
            FriendTag = requestinUser.UserTag.Tag
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);


        // Assert
        result.Success.Should().BeTrue();
        Db.Friendship.FirstOrDefault().Should().BeNull();
    }

    [Fact]
    public async Task DeleteFriendRequestCommandHandler_ShouldReturnError_WhenUserProfileNotFound()
    {
        // Arrange
        var command = new DeleteFriendRequestCommand
        {
            Id = Guid.NewGuid(),
            FriendTag = "NonExistentTag"
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteFriendRequestCommandHandler_ShouldReturnError_WhenFriendTagNotFound()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        Db.UserProfile.Add(userProfile);
        await Db.SaveChangesAsync();

        var command = new DeleteFriendRequestCommand
        {
            Id = userProfile.Id,
            FriendTag = "NonExistentTag"
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Contain("Friendship not found");
    }

    [Fact]
    public async Task DeleteFriendRequestCommandHandler_ShouldReturnError_WhenFriendshipNotPending()
    {
        // Arrange
        var recievingUser = new UserProfile(Guid.NewGuid(), "James", UserTag.Create("James"));
        var requestinUser = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));

        requestinUser.AddFriendship(recievingUser);

        Db.UserProfile.AddRange(recievingUser, requestinUser);
        await Db.SaveChangesAsync();

        Db.Friendship.FirstOrDefault()!.AcceptFriendship();

        await Db.SaveChangesAsync();

        var command = new DeleteFriendRequestCommand
        {
            Id = recievingUser.Id,
            FriendTag = requestinUser.UserTag.Tag
        };

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
}