namespace Social.Infrastructure.Store;

internal interface IBlobStore 
{
    Task PutAsync(Guid Id, Stream content, CancellationToken ct = default);
    void Delete(Guid Id);
    Stream Open(Guid Id);
}
