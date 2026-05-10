namespace Social.Infrastructure.Store;

public interface IBlobStore
{
    Task PutAsync(Guid Id, Stream content, CancellationToken ct = default);
    void Delete(Guid Id);
    Stream Open(Guid Id);
}
