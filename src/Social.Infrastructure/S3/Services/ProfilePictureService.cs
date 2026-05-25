using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Social.Application.Contracts.Services;
using Social.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Social.Infrastructure.S3.Utilities;
using Social.Infrastructure.S3.Validation;
using Social.Infrastructure.Settings;

namespace Social.Infrastructure.S3.Services;

public class ProfilePictureService(
    IAmazonS3 s3Client,
    IOptions<MinioSettings> minioSettings,
    IFileValidator fileValidator)
    : IProfilePictureService
{
    private readonly string _bucketName = minioSettings.Value.ProfilePictureBucket;

    public async Task<Result<string>> UploadProfilePictureAsync(IFormFile file, Guid userId)
    {
        if (!fileValidator.IsValidImageFile(file, out var errorMessage))
            return Result.Fail<string>(Error.Validation($"Value '{errorMessage}' is not valid in this context"));

        var hashedId = HashHelper.GenerateHash(userId.ToString());
        var keyName = $"profile-pictures/{hashedId}{Path.GetExtension(file.FileName)}";

        await DeleteExistingProfilePictures(userId);

        await using var stream = file.OpenReadStream();
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName,
            InputStream = stream,
            ContentType = file.ContentType,
        };

        var response = await s3Client.PutObjectAsync(putRequest);
        return response.HttpStatusCode == HttpStatusCode.OK
            ? Result.Ok(keyName)
            : Result.Fail<string>(Error.Failure($"{response.HttpStatusCode}: Failed to upload profile picture"));
    }

    public async Task<Result<string>> GetPresignedProfilePictureUrlAsync(string profilePictureUrl)
    {
        var getRequest = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = profilePictureUrl,
            Expires = DateTime.UtcNow.AddMinutes(10)
        };

        var url = await s3Client.GetPreSignedURLAsync(getRequest);
        if (url is null)
            return Result.Fail<string>(Error.Failure("Failed to generate presigned URL."));

        var originalUri = new Uri(url);
        var relativeUri = originalUri.PathAndQuery;

        return Result.Ok(relativeUri);
    }

    public async Task<Result> DeleteProfilePictureAsync(string keyName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName,
        };

        var response = await s3Client.DeleteObjectAsync(deleteRequest);
        return response.HttpStatusCode is HttpStatusCode.NoContent or HttpStatusCode.OK
            ? Result.Ok(true)
            : Result.Fail(Error.Failure($"{response.HttpStatusCode}: Failed to delete profile picture"));
    }

    private async Task DeleteExistingProfilePictures(Guid userId)
    {
        var hashedId = HashHelper.GenerateHash(userId.ToString());
        var listRequest = new ListObjectsV2Request()
        {
            BucketName = _bucketName,
            Prefix = $"profile-pictures/{hashedId}"
        };

        var listResponse = await s3Client.ListObjectsV2Async(listRequest);

        if (listResponse.S3Objects is null || listResponse.S3Objects.Count == 0) return;

        var keys = listResponse.S3Objects.Select(o => o.Key);

        foreach (var key in keys)
        {
            await DeleteProfilePictureAsync(key);
        }
    }
}
