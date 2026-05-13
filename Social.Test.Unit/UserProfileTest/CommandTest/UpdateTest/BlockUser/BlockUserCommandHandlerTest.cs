using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Commands.Update.BlockUser;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Services;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.CommandTest.UpdateTest.BlockUser;

public class BlockUserCommandHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IUserBlockingService _fakeService;
    private readonly IUnitOfWork _fakeUow;
    private readonly BlockUserCommandHandler _sut;

    public BlockUserCommandHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeService = A.Fake<IUserBlockingService>();
        _fakeUow = A.Fake<IUnitOfWork>();
        var fakeLogger = A.Fake<ILogger<BlockUserCommandHandler>>();
        _sut = new(_fakeRepo, _fakeService, _fakeUow, fakeLogger);
    }

    [Fact]
    public async Task Handle_WhenUserProfileIsNull_ReturnsNotFound()
    {
        // Arrange
        var request = new BlockUserCommand { TenantId = Guid.Empty, BlockedUserTag = "blockedUserTag" };
        A.CallTo(() => _fakeRepo.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).Returns((UserProfile)null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_WhenBlockedUserProfileIsNull_ReturnsNotFound()
    {
        // Arrange
        var request = new BlockUserCommand { TenantId = Guid.NewGuid(), BlockedUserTag = "blockedUserTag" };
        A.CallTo(() => _fakeRepo.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._))
            .ReturnsNextFromSequence<UserProfile?>(new UserProfile(), null);

        // Act
        var result = await _sut.Handle(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }
}
