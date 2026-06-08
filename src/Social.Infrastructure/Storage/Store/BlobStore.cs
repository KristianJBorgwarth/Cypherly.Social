using Microsoft.Extensions.Options;
using Social.Infrastructure.Settings;

namespace Social.Infrastructure.Storage.Store;

internal sealed class BlobStore(IOptions<BlobStoreSettings> options) : IBlobStore
{
    public async Task PutAsync(Guid id, Stream content, CancellationToken ct = default)
    {
        var path = GeneratePath(id);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        var tmpPath = path + ".tmp-" + Guid.NewGuid().ToString("N");

        try
        {
            using var fileStream = new FileStream(tmpPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(fileStream, ct);
            await fileStream.FlushAsync(ct);
            fileStream.Close();
            File.Move(tmpPath, path, true);
        }
        catch
        {
            if (File.Exists(tmpPath))
            {
                File.Delete(tmpPath);
            }
            throw;
        }
    }

    public Stream Open(Guid id)
    {
        return new FileStream(
            GeneratePath(id),
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);
    }

    public void Delete(Guid id)
    {
        var path = GeneratePath(id);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private string GeneratePath(Guid id)
    {
        var str = id.ToString("N");
        return Path.Combine(options.Value.Root, str.Substring(0, 2), str);
    }
}
