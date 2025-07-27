using Cypherly.UserManagement.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using OutboxMessage = Cypherly.UserManagement.Infrastructure.Persistence.Outbox.OutboxMessage;

namespace Cypherly.UserManagement.Infrastructure.Persistence.Repositories;

public class OutboxRepository(UserManagementDbContext context) : IOutboxRepository
{
    /// <summary>
    /// Get unprocessed outbox messages
    /// </summary>
    /// <param name="batchSize">Amount of outbox messages retrieved</param>
    /// <returns>An Array of <see cref="OutboxMessage"/></returns>
    public async Task<OutboxMessage[]> GetUnprocessedAsync(int batchSize)
    {
        return await context.OutboxMessage
            .Where(m => m.ProcessedOn == null)
            .Take(batchSize)
            .ToArrayAsync();
    }

    /// <summary>
    /// Marks an outbox message as processed by setting the ProcessedOn property to the current date and time.
    /// </summary>
    /// <param name="message">The <see cref="OutboxMessage"/> that will be marked as processed</param>
    /// <returns></returns>
    public Task MarkAsProcessedAsync(OutboxMessage message)
    {
        message.ProcessedOn = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}