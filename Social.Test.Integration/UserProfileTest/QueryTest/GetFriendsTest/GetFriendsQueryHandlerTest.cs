using Social.Infrastructure.Persistence.Context;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Features.UserProfile.Queries.GetFriends;
using Social.Domain.Aggregates;
using Social.Domain.Entities;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.QueryTest.GetFriendsTest;

public class GetFriendsQueryHandlerTest : IntegrationTestBase
{
    private readonly GetFriendsQueryHandler _sut;
    private readonly IConnectionIdProvider _connectionIdProvider = A.Fake<IConnectionIdProvider>();
    public GetFriendsQueryHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<GetFriendsQueryHandler>>();
        var profilePictureService = scope.ServiceProvider.GetRequiredService<IProfilePictureService>();
        _sut = new(repo, _connectionIdProvider, profilePictureService, logger);
    }

    [Fact]
    public async Task Handle_Given_Valid_Query_Should_Return_Relevant_Friends()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        var friendProfile = new UserProfile(Guid.NewGuid(), "John", UserTag.Create("John"));
        var friendship = new Friendship(userProfile.Id, friendProfile.Id);
        friendship.AcceptFriendship();
        Db.Add(userProfile);
        Db.Add(friendProfile);
        Db.Add(friendship);
        await Db.SaveChangesAsync();

        var connectionIds = new Dictionary<Guid, List<Guid>> { { friendProfile.Id, [Guid.NewGuid(), Guid.NewGuid()] } };
        A.CallTo(() => _connectionIdProvider.GetConnectionIdsByUsers(new[] { friendProfile.Id }, CancellationToken.None)).Returns(connectionIds);

        var query = new GetFriendsQuery { TenantId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value.First().Username.Should().Be("John");
        result.Value.First().ConnectionIds.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_Given_Invalid_Query_Should_Return_NotFound()
    {
        // Arrange
        var query = new GetFriendsQuery { TenantId = Guid.NewGuid() };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Code.Should().Be("entity.not.found");
    }

    [Fact]
    public async Task Handle_Given_Empty_Friends_Should_Return_Empty_List()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.Add(userProfile);
        await Db.SaveChangesAsync();

        var query = new GetFriendsQuery { TenantId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(0);
    }
}