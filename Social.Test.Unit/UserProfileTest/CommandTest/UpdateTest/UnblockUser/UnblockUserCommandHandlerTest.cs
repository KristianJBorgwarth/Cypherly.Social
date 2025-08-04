using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Update.UnblockUser;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.Services;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.UnblockUser;

public class UnblockUserCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUow;
    private readonly IUserBlockingService _fakeProfileService;
    private readonly UnblockUserCommandHandler _sut;

    public UnblockUserCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeUow = A.Fake<IUnitOfWork>();
        _fakeProfileService = A.Fake<IUserBlockingService>();
        var fakeLogger = A.Fake<ILogger<UnblockUserCommandHandler>>();
        _sut = new(_fakeRepo, _fakeUow, _fakeProfileService, fakeLogger);
    }

    [Fact]
    public async void Handle_WhenUserProfileIsNull_ReturnsNotFound()
    {
        // Arrange
        var command = new UnblockUserCommand { TenantId = Guid.NewGuid(), Tag = "tag" };
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId))!.Returns<UserProfile>(null);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().BeEquivalentTo(
            Errors.General.NotFound(command.TenantId).Message);
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_When_UserToUnblockIsNull_ReturnsNotFound()
    {
        // Arrange
        var command = new UnblockUserCommand { TenantId = Guid.NewGuid(), Tag = "tag" };
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId))!.Returns(new UserProfile());
        A.CallTo(() => _fakeRepo.GetByUserTag(command.Tag))!.Returns<UserProfile>(null);

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().BeEquivalentTo(
            Errors.General.NotFound(command.Tag).Message);
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).MustNotHaveHappened();
        A.CallTo(() => _fakeProfileService.UnblockUser(A<UserProfile>._, A<UserProfile>._)).MustNotHaveHappened();
    }

    [Fact]
    public async void Handle_When_Command_IsValid_Should_Unblock_And_Return_ResultOK()
    {
        // Arrange
        var command = new UnblockUserCommand { TenantId = Guid.NewGuid(), Tag = "tag" };
        var userProfile = new UserProfile();
        var userToUnblock = new UserProfile();
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId))!.Returns(userProfile);
        A.CallTo(() => _fakeRepo.GetByUserTag(command.Tag))!.Returns(userToUnblock);
        A.CallTo(() => _fakeProfileService.UnblockUser(userProfile, userToUnblock)).DoesNothing();

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.Success.Should().BeTrue();
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).MustHaveHappened();
        A.CallTo(() => _fakeProfileService.UnblockUser(userProfile, userToUnblock)).MustHaveHappened();
    }

    [Fact]
    public async void Handle_When_Command_IsValid_And_Exception_IsThrown_Should_Return_UnspecifiedError()
    {
        // Arrange
        var command = new UnblockUserCommand { TenantId = Guid.NewGuid(), Tag = "tag" };
        var userProfile = new UserProfile();
        var userToUnblock = new UserProfile();
        A.CallTo(() => _fakeRepo.GetByIdAsync(command.TenantId))!.Returns(userProfile);
        A.CallTo(() => _fakeRepo.GetByUserTag(command.Tag))!.Returns(userToUnblock);
        A.CallTo(() => _fakeProfileService.UnblockUser(userProfile, userToUnblock)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(command, default);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().BeEquivalentTo(
            Errors.General.UnspecifiedError("An exception occured while attempting to unblock").Message);
        A.CallTo(() => _fakeUow.SaveChangesAsync(default)).MustNotHaveHappened();
    }
}