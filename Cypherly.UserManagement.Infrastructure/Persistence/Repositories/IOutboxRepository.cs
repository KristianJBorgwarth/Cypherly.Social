using OutboxMessage = Cypherly.UserManagement.Infrastructure.Persistence.Outbox.OutboxMessage;

namespace Cypherly.UserManagement.Infrastructure.Persistence.Repositories;

public interface IOutboxRepository
{
    Task<OutboxMessage[]> GetUnprocessedAsync(int batchSize);
    Task MarkAsProcessedAsync(OutboxMessage message);
}