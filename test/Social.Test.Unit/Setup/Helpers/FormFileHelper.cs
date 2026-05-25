using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Social.Test.Unit.Setup.Helpers;

public static class FormFileHelper
{
    public static IFormFile CreateFormFile(string filePath, string contentType)
    {
        var fileInfo = new FileInfo(filePath);
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return new FormFile(stream, 0, fileInfo.Length, "file", fileInfo.Name)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
            ContentDisposition = $"form-data; name=\"file\"; filename=\"{fileInfo.Name}\""
        };
    }
}
