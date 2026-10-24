using Social.Infrastructure.Persistence.Context;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.Friendships.Queries.GetFriends;
using Social.Domain.Aggregates;
using Social.Domain.Entities;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.QueryTest.GetFriendsTest;

public class GetFriendsQueryHandlerTest : IntegrationTestBase
{
    private readonly GetFriendsQueryHandler _sut;
    private readonly IConnectionIdProvider _fakeProvider;
    public GetFriendsQueryHandlerTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        _fakeProvider = A.Fake<IConnectionIdProvider>();
        _sut = new(repo, _fakeProvider);
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

        var query = new GetFriendsQuery { TenantId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(1);
        result.Value.First().Username.Should().Be("John");
    }

    [Fact]
    public async Task Handle_Given_Friends_Should_Return_Their_Connection_Ids()
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

        var connectionId = Guid.NewGuid();
        A.CallTo(() => _fakeProvider.GetConnectionIdsMultipleTenants(A<Guid[]>._, A<CancellationToken>._))
            .Returns(new Dictionary<Guid, List<Guid>> { [friendProfile.Id] = [connectionId] });

        var query = new GetFriendsQuery { TenantId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Value!.Single().ConnectionIds.Should().Equal(connectionId);
        A.CallTo(() => _fakeProvider.GetConnectionIdsMultipleTenants(
                A<Guid[]>.That.Matches(ids => ids.Length == 1 && ids[0] == friendProfile.Id), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Handle_Given_Friend_Unknown_To_Identity_Should_Return_No_Connection_Ids()
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

        A.CallTo(() => _fakeProvider.GetConnectionIdsMultipleTenants(A<Guid[]>._, A<CancellationToken>._))
            .Returns(new Dictionary<Guid, List<Guid>>());

        var query = new GetFriendsQuery { TenantId = userProfile.Id };

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Single().ConnectionIds.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Given_No_Friends_Should_Not_Ask_For_Connection_Ids()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "David", UserTag.Create("David"));
        Db.Add(userProfile);
        await Db.SaveChangesAsync();

        // Act
        await _sut.Handle(new GetFriendsQuery { TenantId = userProfile.Id }, CancellationToken.None);

        // Assert
        A.CallTo(() => _fakeProvider.GetConnectionIdsMultipleTenants(A<Guid[]>._, A<CancellationToken>._)).MustNotHaveHappened();
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
        result.Error!.Code.Should().Be("entity.not.found");
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
