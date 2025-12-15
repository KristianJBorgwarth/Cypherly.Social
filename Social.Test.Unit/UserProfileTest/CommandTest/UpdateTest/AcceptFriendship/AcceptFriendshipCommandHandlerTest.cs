using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Commands.Update.AcceptFriendship;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.Interfaces;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.AcceptFriendship;

public class AcceptFriendshipCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IFriendshipService _fakeService;
    private readonly IUnitOfWork _fakeUow;
    private readonly IConnectionIdProvider _fakeProvider;
    private readonly IProfilePictureService _fakePictureService;
    private readonly AcceptFriendshipCommandHandler _sut;

    public AcceptFriendshipCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeService = A.Fake<IFriendshipService>();
        _fakeUow = A.Fake<IUnitOfWork>();
        _fakePictureService = A.Fake<IProfilePictureService>();
        _fakeProvider = A.Fake<IConnectionIdProvider>();
        var fakeLogger = A.Fake<ILogger<AcceptFriendshipCommandHandler>>();
        _sut = new AcceptFriendshipCommandHandler(_fakeRepo, _fakeService, _fakePictureService, _fakeProvider, _fakeUow, fakeLogger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Command_Should_Create_Friendship_And_Return_ResultOk()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var friendProfile = new UserProfile(Guid.NewGuid(), "Friend", UserTag.Create("Friend"));
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "friend",
            TenantId = userProfile.Id
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).Returns(userProfile);
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).Returns(Result.Ok());
        A.CallTo(() => _fakeRepo.UpdateAsync(userProfile)).Returns(Task.CompletedTask);
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);
        A.CallTo(() => _fakeRepo.GetByUserTag(command.FriendTag)).Returns(friendProfile);
        A.CallTo(() => _fakeProvider.GetConnectionIdsSingleTenant(friendProfile.Id, CancellationToken.None)).Returns([Guid.NewGuid(), Guid.NewGuid()]);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.DisplayName.Should().BeEquivalentTo("Friend");
        result.Value.ProfilePictureUrl.Should().BeNullOrEmpty();
        result.Value.ConnectionIds.Should().HaveCount(2);
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(userProfile)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Given_Invalid_Id_Should_Return_ResultFail()
    {
        // Arrange
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "friend",
            TenantId = Guid.NewGuid()
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId)).Returns((UserProfile)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(command.TenantId));
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.AcceptFriendship(A<UserProfile>._, A<string>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeRepo.UpdateAsync(A<UserProfile>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Invalid_FriendTag_Should_Return_ResultFail()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var command = new AcceptFriendshipCommand()
        {
            FriendTag = "friend",
            TenantId = userProfile.Id
        };
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).Returns(userProfile);
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).Returns(Result.Fail(Errors.General.UnspecifiedError("Friendship not found")));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.UnspecifiedError("Friendship not found"));
        A.CallTo(() => _fakeRepo.GetByIdAsync(userProfile.Id)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeService.AcceptFriendship(userProfile, command.FriendTag)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(A<UserProfile>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUow.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }
}
