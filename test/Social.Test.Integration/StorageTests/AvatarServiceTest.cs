using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Contracts.Services;
using Social.Infrastructure.Persistence.Context;
using Social.Test.Integration.Setup;
using Social.Test.Integration.Setup.Helpers;
using Xunit;

namespace Social.Test.Integration.StorageTests;

public class AvatarServiceTest : IntegrationTestBase
{
    private readonly IAvatarService _sut;
    private readonly string testFilePath = "test/Social.Test.Integration/Setup/Resources/test_profile_picture.png";

    public AvatarServiceTest(IntegrationTestFactory<Program, SocialDbContext> factory) : base(factory)
    {
        var scope = factory.Services.CreateScope();
        _sut = scope.ServiceProvider.GetRequiredService<IAvatarService>();
    }

    [Fact]
    public async Task UploadAvatar_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var filePath = Path.Combine(DirectoryHelper.GetProjectRootDirectory(), testFilePath);

        var formFile = FormFileHelper.CreateFormFile(filePath, "image/png");

        // Act
        var result = await _sut.UploadAsync(formFile, userId, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ContentType.Should().Be("image/png");
        result.Value.Stream.Should().NotBeNull();
    }
}