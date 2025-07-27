using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Social.Infrastructure.S3.Services;
using Social.Infrastructure.S3.Utilities;
using Social.Infrastructure.S3.Validation;
using Social.Infrastructure.Settings;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Social.Domain.Common;
using Social.Test.Integration.Setup.Helpers;

namespace Social.Test.Integration.BucketTest;

public class ProfilePictureServiceTests
{
    private readonly ProfilePictureService _profilePictureService;
    private readonly IAmazonS3 _fakeS3Client;
    private readonly IFileValidator _fakeFileValidator;

    public ProfilePictureServiceTests()
    {
        _fakeFileValidator = A.Fake<IFileValidator>();
        var fakeMinioSettings = A.Fake<IOptions<MinioSettings>>();
        _fakeS3Client = A.Fake<AmazonS3Client>();

        _profilePictureService = new(_fakeS3Client, fakeMinioSettings, _fakeFileValidator);
    }

    [Fact]
    public async Task UploadProfilePictureAsync_Valid_Picture_Should_Upload_And_Return_Key()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileContent = new byte[] { 0x01, 0x02, 0x03 };
        var file = MockHelper.CreateFakeIFormFile("test.jpg", "image/jpeg", fileContent);
        string errorMessage;
        A.CallTo(() => _fakeFileValidator.IsValidImageFile(file, out errorMessage)).Returns(true);

        var putObjectResponse = new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK };
        A.CallTo(() => _fakeS3Client.PutObjectAsync(A<PutObjectRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(putObjectResponse));

        var hashedId = HashHelper.GenerateHash(userId.ToString());
        var expectedKey = $"profile-pictures/{hashedId}.jpg";

        // Act
        var result = await _profilePictureService.UploadProfilePictureAsync(file, userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be(expectedKey); // match the hashed key now
    }


    [Fact]
    public async Task UploadProfilePictureAsync_Invalid_Picture_Should_ReturnFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileContent = new byte[] { 0x01, 0x02, 0x03 };
        var file = MockHelper.CreateFakeIFormFile("test.jpg", "image/jpeg", fileContent);
        var errorMessage = "Invalid file format.";
        A.CallTo(() => _fakeFileValidator.IsValidImageFile(file, out errorMessage)).Returns(false);

        // Act
        var result = await _profilePictureService.UploadProfilePictureAsync(file, userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.UnexpectedValue(errorMessage).Message);
    }

    [Fact]
    public async Task UploadProfilePictureAsync_S3_Failure_Should_ReturnFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileContent = new byte[] { 0x01, 0x02, 0x03 };
        var file = MockHelper.CreateFakeIFormFile("test.jpg", "image/jpeg", fileContent);
        string errorMessage;
        A.CallTo(() => _fakeFileValidator.IsValidImageFile(file, out errorMessage)).Returns(true);

        var putObjectResponse = new PutObjectResponse { HttpStatusCode = HttpStatusCode.InternalServerError };
        A.CallTo(() => _fakeS3Client.PutObjectAsync(A<PutObjectRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(putObjectResponse));

        // Act
        var result = await _profilePictureService.UploadProfilePictureAsync(file, userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.UnspecifiedError($"{HttpStatusCode.InternalServerError}: Failed to upload profile picture").Message);
    }


    [Fact]
    public async Task DeleteProfilePictureAsync_Valid_Delete_Should_Delete_And_Return_ResultOk()
    {
        // Arrange
        const string keyName = "profile-pictures/test.jpg";
        var deleteObjectResponse = new DeleteObjectResponse { HttpStatusCode = HttpStatusCode.NoContent };
        A.CallTo(() => _fakeS3Client.DeleteObjectAsync(A<DeleteObjectRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(deleteObjectResponse));

        // Act
        var result = await _profilePictureService.DeleteProfilePictureAsync(keyName);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteProfilePictureAsync_Invalid_Delete_Should_Return_ResultFail()
    {
        // Arrange
        const string keyName = "profile-pictures/test.jpg";
        var deleteObjectResponse = new DeleteObjectResponse { HttpStatusCode = HttpStatusCode.InternalServerError };
        A.CallTo(() => _fakeS3Client.DeleteObjectAsync(A<DeleteObjectRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(deleteObjectResponse));

        // Act
        var result = await _profilePictureService.DeleteProfilePictureAsync(keyName);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.UnspecifiedError($"{HttpStatusCode.InternalServerError}: Failed to delete profile picture").Message);
    }
}