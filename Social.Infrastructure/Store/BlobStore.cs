namespace Social.Infrastructure.Store;

public sealed class BlobStore(string root) : IBlobStore
{
    public async Task PutAsync(Guid Id, Stream content, CancellationToken ct = default)
    {
        var path = GeneratePath(Id);
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

    public Stream Open(Guid Id)
    {
        return new FileStream(
            GeneratePath(Id),
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);
    }

    public void Delete(Guid Id)
    {
        var path = GeneratePath(Id);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private string GeneratePath(Guid id)
    {
        var str = id.ToString("N");
        return Path.Combine(root, str.Substring(0, 2), str);
    }
}
