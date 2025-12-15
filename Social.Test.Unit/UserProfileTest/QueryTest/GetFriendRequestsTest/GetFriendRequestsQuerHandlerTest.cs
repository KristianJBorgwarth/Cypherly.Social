using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetFriendRequests;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Xunit;

namespace Social.Test.Unit.UserProfileTest.QueryTest.GetFriendRequestsTest;

public class GetFriendRequestsQuerHandlerTest
{
    private readonly IUserProfileRepository _fakeRepo;
    private readonly GetFriendRequestsQueryHandler _sut;

    public GetFriendRequestsQuerHandlerTest()
    {
        _fakeRepo = A.Fake<IUserProfileRepository>();
        var logger = A.Fake<ILogger<GetFriendRequestsQueryHandler>>();
        var profilePictureService = A.Fake<IProfilePictureService>();
        _sut = new GetFriendRequestsQueryHandler(_fakeRepo, profilePictureService, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Query_Should_Return_Result_Ok()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "Kristian", UserTag.Create("Kristian"));

        var query = new GetFriendRequestsQuery()
        {
            TenantId = user.Id,
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(user.Id)).Returns(user);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact]
    public async Task Handle_Given_Invalid_Query_Should_Return_Result_Fail()
    {
        // Arrange
        var query = new GetFriendRequestsQuery()
        {
            TenantId = Guid.NewGuid(),
        };

        A.CallTo(() => _fakeRepo.GetByIdAsync(query.TenantId)).Returns((UserProfile)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }
}
