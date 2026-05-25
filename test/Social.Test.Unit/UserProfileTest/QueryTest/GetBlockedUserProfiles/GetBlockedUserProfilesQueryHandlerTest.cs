using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Application.Features.Friendships.Queries.GetBlockedUserProfiles;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetBlockedUserProfiles;

public class GetBlockedUserProfilesQueryHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly GetBlockedUserProfilesQueryHandler _sut;

    public GetBlockedUserProfilesQueryHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        _sut = new GetBlockedUserProfilesQueryHandler(_fakeRepo, A.Fake<ILogger<GetBlockedUserProfilesQueryHandler>>());
    }

    [Fact]
    public async Task Handle_WhenUserProfileNotFound_ReturnsNotFound()
    {
        // Arrange
        var query = new GetBlockedUserProfilesQuery { TenantId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetSingleAsync(A<ISpecification<UserProfile>>._, A<CancellationToken>._)).Returns((UserProfile?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Error.NotFound<UserProfile>(query.TenantId.ToString()));
    }
}
