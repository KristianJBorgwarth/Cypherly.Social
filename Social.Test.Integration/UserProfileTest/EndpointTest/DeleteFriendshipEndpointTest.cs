using System.Net;
using System.Net.Http.Json;
using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Social.API.Common;
using Social.Application.Features.UserProfile.Commands.Delete.Friendship;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;
using Social.Test.Integration.Setup.Attributes;

namespace Social.Test.Integration.UserProfileTest.EndpointTest;

public class DeleteFriendshipEndpointTest(IntegrationTestFactory<Program, SocialDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async Task Given_Valid_Request_Should_Delete_Friendship_And_Return_200OK()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));

        var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

        userProfile.AddFriendship(friendProfile);
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        await Db.SaveChangesAsync();
        Db.Friendship.Should().HaveCount(1);

        var cmd = new DeleteFriendshipCommand
        {
            TenantId = userProfile.Id,
            FriendTag = friendProfile.UserTag.Tag
        };

        // Act
        var encodedFriendTag = Uri.EscapeDataString(cmd.FriendTag);
        var response = await Client.DeleteAsync($"api/userprofile/friendship/delete?Id={cmd.TenantId}&friendTag={encodedFriendTag}");

        // Assert
        var result = await response.Content.ReadFromJsonAsync<Envelope>();
        result.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.Friendship.Should().HaveCount(0);
    }

    [SkipOnGitHubFact]
    public async Task Given_Invalid_Request_Should_Return_400BadRequest()
    {
        // Arrange
        var userProfile = new UserProfile(Guid.NewGuid(), "TestUser", UserTag.Create("TestUser"));

        var friendProfile = new UserProfile(Guid.NewGuid(), "FriendUser", UserTag.Create("FriendUser"));

        userProfile.AddFriendship(friendProfile);
        Db.UserProfile.Add(userProfile);
        Db.UserProfile.Add(friendProfile);
        await Db.SaveChangesAsync();
        Db.Friendship.Should().HaveCount(1);

        var cmd = new DeleteFriendshipCommand
        {
            TenantId = userProfile.Id,
            FriendTag = "InvalidFriendTag"
        };

        // Act
        var encodedFriendTag = Uri.EscapeDataString(cmd.FriendTag);
        var response = await Client.DeleteAsync($"api/userprofile/friendship/delete?Id={cmd.TenantId}&friendTag={encodedFriendTag}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.Friendship.Should().HaveCount(1);
    }
}