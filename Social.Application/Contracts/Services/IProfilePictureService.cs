using Cypherly.UserManagement.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Social.Application.Contracts.Services;

public interface IProfilePictureService
{
    Task<Result<string>> UploadProfilePictureAsync(IFormFile file, Guid userId);
    Task<Result<string>> GetPresignedProfilePictureUrlAsync(string keyName);
    Task<Result> DeleteProfilePictureAsync(string keyName);
}