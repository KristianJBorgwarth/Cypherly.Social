using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Update.DisplayName;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.DisplayName;

public class UpdateUserProfileDisplayNameCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUnitOfWork _fakeUnitOfWork;
    private readonly UpdateUserProfileDisplayNameCommandHandler _sut;

    public UpdateUserProfileDisplayNameCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        var fakeLogger = A.Fake<ILogger<UpdateUserProfileDisplayNameCommandHandler>>();
        _fakeUnitOfWork = A.Fake<IUnitOfWork>();
        _sut = new(_fakeRepo, fakeLogger, _fakeUnitOfWork);
    }

    [Fact]
    public async Task Handle_Given_ValidCommand_Should_ReturnDto_And_Result_Ok()
    {
        // Arrange
        var testProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var cmd = new UpdateUserProfileDisplayNameCommand
        {
            DisplayName = "validDisplayName",
            TenantId = testProfile.Id
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.TenantId, A<CancellationToken>._)).Returns(testProfile);
        A.CallTo(() => _fakeRepo.UpdateAsync(testProfile, A<CancellationToken>._)).DoesNothing();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(default)).DoesNothing();
        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.DisplayName.Should().Be(cmd.DisplayName);
        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.TenantId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(testProfile, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(default)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Given_Command_With_Invalid_UserProfileId_Should_Return_Result_Fail()
    {
        // Arrrange
        var cmd = new UpdateUserProfileDisplayNameCommand
        {
            DisplayName = "validDisplayName",
            TenantId = Guid.NewGuid()
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.TenantId, A<CancellationToken>._)).Returns((UserProfile)null);

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeFalse();
        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.TenantId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(A<UserProfile>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
    }

    [Fact]
    public async Task Handle_Given_Command_With_Invalid_DisplayName_Should_Return_Result_Fail()
    {
        // Arrange
        var testProfile = new UserProfile(Guid.NewGuid(), "dave", UserTag.Create("dave"));
        var cmd = new UpdateUserProfileDisplayNameCommand
        {
            DisplayName = "", // invalid
            TenantId = testProfile.Id
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.TenantId, A<CancellationToken>._)).Returns(testProfile);

        // Act
        var result = await _sut.Handle(cmd, default);

        // Assert
        result.Success.Should().BeFalse();
        A.CallTo(() => _fakeRepo.GetByIdAsync(cmd.TenantId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fakeRepo.UpdateAsync(A<UserProfile>._, A<CancellationToken>._)).MustNotHaveHappened();
        A.CallTo(() => _fakeUnitOfWork.SaveChangesAsync(default)).MustNotHaveHappened();
    }
}
