using System.Net;
using System.Net.Http.Json;
using Social.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Social.Application.Features.UserProfile.Commands.Update.DisplayName;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Test.Integration.Setup;
using Social.Test.Integration.Setup.Attributes;

namespace Social.Test.Integration.UserProfileTest.EndpointTest;

public class UpdateUserProfileDisplayNameEndpointTest(IntegrationTestFactory<Program, UserManagementDbContext> factory)
    : IntegrationTestBase(factory)
{
    [SkipOnGitHubFact]
    public async Task Given_Valid_Request_Should_Update_UserProfile_DisplayName_And_Return_200OK()
    {
        // Arrange
        var userprofile = new UserProfile(Guid.NewGuid(), "david", UserTag.Create("david"));
        Db.UserProfile.Add(userprofile);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfileDisplayNameCommand()
        {
            DisplayName = "test",
            Id = userprofile.Id
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/userprofile/displayname", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        Db.UserProfile.AsNoTracking().FirstOrDefault(u => u.Id == userprofile.Id)!.DisplayName.Should().Be("test");
        Db.OutboxMessage.Should().HaveCount(1);
    }

    [SkipOnGitHubFact]
    public async Task Given_Invalid_Request_Should_Return_400BadRequest()
    {
        // Arrange
        var userprofile = new UserProfile(Guid.NewGuid(), "david", UserTag.Create("david"));
        Db.UserProfile.Add(userprofile);
        await Db.SaveChangesAsync();

        var command = new UpdateUserProfileDisplayNameCommand()
        {
            DisplayName = "",
            Id = Guid.NewGuid()
        };

        // Act
        var response = await Client.PutAsJsonAsync("/api/userprofile/displayname", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        Db.UserProfile.AsNoTracking().FirstOrDefault(u => u.Id == userprofile.Id)!.DisplayName.Should().Be("david");
    }
}