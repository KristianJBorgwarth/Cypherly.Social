using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Infrastructure.Persistence.Context;
using Social.Test.Integration.Setup;

namespace Social.Test.Integration.UserProfileTest.SpecificationTest;

public class FriendshipSpecsAvatarTest : IntegrationTestBase
{
    private readonly IUserProfileRepository _repo;

    public FriendshipSpecsAvatarTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _repo = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
    }

    private async Task<(UserProfile user, UserProfile requester)> SeedRequesterWithAvatarAsync()
    {
        var user = new UserProfile(Guid.NewGuid(), "Kristian", UserTag.Create("Kristian"));
        var requester = new UserProfile(Guid.NewGuid(), "Friend", UserTag.Create("Friend"));
        requester.GetOrCreateAvatar("image/png");

        Db.Add(user);
        Db.Add(requester);
        requester.AddFriendship(user);
        await Db.SaveChangesAsync();

        return (user, requester);
    }

    [Fact]
    public async Task UserProfileByTagWithFriendshipsSpec_With_IncludeAvatar_Should_Load_Avatar()
    {
        // Arrange
        var (_, requester) = await SeedRequesterWithAvatarAsync();

        // Act
        var profile = await _repo.GetSingleAsync(new UserProfileByTagWithFriendshipsSpec(requester.UserTag.Tag, includeAvatar: true), CancellationToken.None);

        // Assert
        profile!.Avatar.Should().NotBeNull();
        profile.Avatar!.FileKey.Should().Be(requester.Avatar!.FileKey);
    }

    [Fact]
    public async Task UserProfileByTagWithFriendshipsSpec_By_Default_Should_Not_Load_Avatar()
    {
        // Arrange
        var (_, requester) = await SeedRequesterWithAvatarAsync();

        // Act
        var profile = await _repo.GetSingleAsync(new UserProfileByTagWithFriendshipsSpec(requester.UserTag.Tag), CancellationToken.None);

        // Assert
        profile!.Avatar.Should().BeNull();
    }

    [Fact]
    public async Task UserProfileWithFriendRequestsSpec_By_Default_Should_Not_Load_Requester_Avatar()
    {
        // Arrange
        var (user, _) = await SeedRequesterWithAvatarAsync();

        // Act
        var profile = await _repo.GetSingleAsync(new UserProfileWithFriendRequestsSpec(user.Id), CancellationToken.None);

        // Assert
        profile!.FriendshipsReceived.Should().ContainSingle().Which.UserProfile.Avatar.Should().BeNull();
    }
}
