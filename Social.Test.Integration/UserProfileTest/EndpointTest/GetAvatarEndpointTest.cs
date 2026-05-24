using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Services;
using Social.Domain.Aggregates;
using Social.Domain.ValueObjects;
using Social.Infrastructure.Persistence.Context;
using Social.Test.Integration.Setup;
using Social.Test.Integration.Setup.Helpers;

namespace Social.Test.Integration.UserProfileTest.EndpointTest;

public class GetAvatarEndpointTest : IntegrationTestBase
{
    private readonly HttpClient _client;
    private readonly IAvatarService _avatarService;

    public GetAvatarEndpointTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _avatarService = scope.ServiceProvider.GetRequiredService<IAvatarService>();
    }

    [Fact]
    public async Task GetAvatar_WithValidFileKey_ShouldReturn200AndFileStream()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserProfile(userId, "TestUser", UserTag.Create("TestUser"));
        user.GetOrCreateAvatar("image/png");
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();
        var testpicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Social.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png");

        var fk = user.Avatar!.FileKey;
        await _avatarService.UploadAsync(testpicture, fk, CancellationToken.None);

        // Act
        var response = await _client.GetAsync($"/api/userprofile/avatar?FileKey={fk}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("image/png");
        var contentStream = await response.Content.ReadAsStreamAsync();
        contentStream.Should().NotBeNull();
        response.Headers.ETag.Should().NotBeNull();
        response.Headers.ETag!.Tag.Should().NotBeNullOrEmpty();
        response.Headers.ETag!.Tag.Should().Be(user.Avatar.Etag.Value);
    }

    [Fact]
    public async Task GetAvatar_WithInvalidFileKey_ShouldReturn404()
    {
        // Arrange
        var invalidFileKey = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/userprofile/avatar?FileKey={invalidFileKey}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAvatar_WithValidETag_ShouldReturn304NotModified()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserProfile(userId, "TestUser2", UserTag.Create("TestUser2"));
        user.GetOrCreateAvatar("image/png");
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();
        var testpicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Social.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png");

        var fk = user.Avatar!.FileKey;
        await _avatarService.UploadAsync(testpicture, fk, CancellationToken.None);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/userprofile/avatar?FileKey={fk}");
        request.Headers.IfNoneMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue(user.Avatar.Etag.Value));

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotModified);
    }

    [Fact]
    public async Task GetAvatar_WithInvalidETag_ShouldReturn200AndFileStream()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserProfile(userId, "TestUser3", UserTag.Create("TestUser3"));
        user.GetOrCreateAvatar("image/png");
        Db.UserProfile.Add(user);
        await Db.SaveChangesAsync();
        var testpicture = FormFileHelper.CreateFormFile(Path.Combine(DirectoryHelper.GetProjectRootDirectory(), "Social.Test.Integration/Setup/Resources/test_profile_picture.png"), "image/png");

        var fk = user.Avatar!.FileKey;
        await _avatarService.UploadAsync(testpicture, fk, CancellationToken.None);

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/userprofile/avatar?FileKey={fk}");
        request.Headers.IfNoneMatch.Add(new System.Net.Http.Headers.EntityTagHeaderValue(ETag.Generate().Value));

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("image/png");
        var contentStream = await response.Content.ReadAsStreamAsync();
        contentStream.Should().NotBeNull();
        response.Headers.ETag.Should().NotBeNull();
        response.Headers.ETag!.Tag.Should().NotBeNullOrEmpty();
        response.Headers.ETag!.Tag.Should().Be(user.Avatar.Etag.Value);
    }
}
