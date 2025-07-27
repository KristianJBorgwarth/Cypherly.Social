namespace Social.Application.Contracts.Clients;

public interface IMinioProxyClient
{
    public Task<(byte[] image, string imageType)?> GetImageFromMinioAsync(string url, CancellationToken cancellationToken = default);
}