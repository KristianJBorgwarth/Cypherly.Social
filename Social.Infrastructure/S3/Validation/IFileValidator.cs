using Microsoft.AspNetCore.Http;

namespace Social.Infrastructure.S3.Validation;

public interface IFileValidator
{
    bool IsValidImageFile(IFormFile file, out string errorMessage);
}