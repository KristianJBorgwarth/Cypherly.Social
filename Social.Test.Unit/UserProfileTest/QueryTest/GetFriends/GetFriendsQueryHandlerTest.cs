using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using FakeItEasy;
using FluentAssertions;
using Social.Application.Features.Friendships.Queries.GetFriends;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetFriends;

public class GetFriendsQueryHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly GetFriendsQueryHandler _sut;

    public GetFriendsQueryHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _sut = new GetFriendsQueryHandler(_fakeRepo);
    }

    [Fact]
    public async Task Handle_WhenUserProfileDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var query = new GetFriendsQuery { TenantId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).Returns((UserProfile)null!);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("entity.not.found");
        A.CallTo(() => _fakeRepo.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_WhenUserProfileExists_ReturnsFriends()
    {
        // Arrange
        var query = new GetFriendsQuery { TenantId = Guid.NewGuid() };
        var userProfile = new UserProfile(Guid.NewGuid(), "Eric", UserTag.Create("Eric"));
        A.CallTo(() => _fakeRepo.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).Returns(userProfile);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        A.CallTo(() => _fakeRepo.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }
}
