using Microsoft.AspNetCore.Http;

namespace Social.Infrastructure.Storage.Validation;

public interface IFileValidator
{
    bool IsValidImageFile(IFormFile file, out string errorMessage);
}
