namespace Social.Infrastructure.Storage.Store;

public interface IBlobStore
{
    Task PutAsync(Guid id, Stream content, CancellationToken ct = default);
    void Delete(Guid Id);
    Stream Open(Guid Id);
}
