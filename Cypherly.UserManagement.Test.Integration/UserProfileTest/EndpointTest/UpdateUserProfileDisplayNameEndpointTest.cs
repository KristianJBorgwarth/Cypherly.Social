using System.Net;
using System.Net.Http.Json;
using Cypherly.UserManagement.Domain.Aggregates;
using Cypherly.UserManagement.Domain.ValueObjects;
using Social.Infrastructure.Persistence.Context;
using Cypherly.UserManagement.Test.Integration.Setup;
using Cypherly.UserManagement.Test.Integration.Setup.Attributes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Social.Application.Features.UserProfile.Commands.Update.DisplayName;

namespace Cypherly.UserManagement.Test.Integration.UserProfileTest.EndpointTest;

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