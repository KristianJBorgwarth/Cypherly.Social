using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Xunit;

namespace Cypherly.UserManagement.Test.Unit.UserProfileTest.QueryTest.GetBlockedUserProfiles;

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
        var query = new GetBlockedUserProfilesQuery { UserId = Guid.NewGuid() };
        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).Returns((UserProfile?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(query.UserId));
    }

    [Fact]
    public async Task Handle_Query_When_Exception_Occurs_Should_Return_ResultFail()
    {
        // Arrange
        var query = new GetBlockedUserProfilesQuery() { UserId = Guid.NewGuid() };

        A.CallTo(() => _fakeRepo.GetByIdAsync(query.UserId)).Throws<Exception>();

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}