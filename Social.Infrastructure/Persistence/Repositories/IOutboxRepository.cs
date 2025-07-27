using Outbox_OutboxMessage = Social.Infrastructure.Persistence.Outbox.OutboxMessage;
using OutboxMessage = Social.Infrastructure.Persistence.Outbox.OutboxMessage;

namespace Social.Infrastructure.Persistence.Repositories;

public interface IOutboxRepository
{
    Task<Outbox_OutboxMessage[]> GetUnprocessedAsync(int batchSize);
    Task MarkAsProcessedAsync(Outbox_OutboxMessage message);
}