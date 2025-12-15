using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetFriendRequests;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.QueryTest.GetFriendRequestsTest;

public class GetFriendRequestsQueryHandlerTest : IntegrationTestBase
{
    private readonly GetFriendRequestsQueryHandler _sut;

    public GetFriendRequestsQueryHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetFriendRequestsQueryHandler>>();
        var profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();

        _sut = new GetFriendRequestsQueryHandler(repo, profilePictureService, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Query_Should_Return_FriendRequests()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "Kristian", UserTag.Create("Kristian"));
        var friend = new UserProfile(Guid.NewGuid(), "Friend", UserTag.Create("Friend"));
        var friend2 = new UserProfile(Guid.NewGuid(), "Friend2", UserTag.Create("Friend2"));

        Db.Add(user);
        Db.Add(friend);
        Db.Add(friend2);
        friend.AddFriendship(user);
        friend2.AddFriendship(user);
        await Db.SaveChangesAsync();

        var query = new GetFriendRequestsQuery()
        {
            TenantId = user.Id,
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeEmpty().And.HaveCount(2);
        result.Value.Should().ContainSingle(f => f.Username == "Friend");
        result.Value.Should().ContainSingle(f => f.Username == "Friend2");
    }

    [Fact]
    public async Task Handle_Given_Valid_Query_But_No_Friends_Should_Return_Empty_List()
    {
        // Arrange
        var user = new UserProfile(Guid.NewGuid(), "Kristian", UserTag.Create("Kristian"));

        Db.Add(user);
        await Db.SaveChangesAsync();

        var query = new GetFriendRequestsQuery()
        {
            TenantId = user.Id,
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Given_Invalid_Query_Should_Return_NotFound()
    {
        // Arrange
        var query = new GetFriendRequestsQuery()
        {
            TenantId = Guid.NewGuid(),
        };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Should().BeEquivalentTo(Errors.General.NotFound(query.TenantId));
    }
}
