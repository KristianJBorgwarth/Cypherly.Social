public static class HttpRequestExtensions
{
    public static string? GetETag(this HttpRequest request)
    {
        var tag = request.GetTypedHeaders().IfNoneMatch?.FirstOrDefault()?.Tag ?? default;
        if (!tag.HasValue || tag.Length < 2) return null;

        return tag.Value;
    }
}
