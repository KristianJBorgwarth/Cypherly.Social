using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetFriends;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetFriends;

public class GetFriendsQueryHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly IConnectionIdProvider _fakeConnectionIdProvider;
    private readonly GetFriendsQueryHandler _sut;

    public GetFriendsQueryHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _fakeConnectionIdProvider = A.Fake<IConnectionIdProvider>();
        var fakeLogger = A.Fake<ILogger<GetFriendsQueryHandler>>();
        var fakeProfilePictureService = A.Fake<IProfilePictureService>();
        _sut = new GetFriendsQueryHandler(_fakeRepo, _fakeConnectionIdProvider, fakeProfilePictureService, fakeLogger);
    }

    [Fact]
    public async void Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetFriendsQuery { TenantId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.TenantId)).Returns((UserProfile)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");

        A.CallTo(() => _fakeRepo.GetByIdAsync(query.TenantId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenExceptionOccurs_ReturnsUnspecifiedError()
    {
        // Arrange
        var query = new GetFriendsQuery { TenantId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.TenantId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Contain("An exception occurred while attempting to retrieve friends.");
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.TenantId)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void Handle_WhenUserProfileExists_ReturnsFriends()
    {
        // Arrange
        var query = new GetFriendsQuery { TenantId = Guid.NewGuid() };
        var userProfile = new UserProfile(Guid.NewGuid(), "Eric", UserTag.Create("Eric"));
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.TenantId)).Returns(userProfile);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.TenantId)).MustHaveHappenedOnceExactly();
    }
}