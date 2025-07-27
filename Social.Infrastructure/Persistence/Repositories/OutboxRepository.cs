using Microsoft.EntityFrameworkCore;
using Social.Infrastructure.Persistence.Context;
using Social.Infrastructure.Persistence.Outbox;
using Outbox_OutboxMessage = Social.Infrastructure.Persistence.Outbox.OutboxMessage;
using OutboxMessage = Social.Infrastructure.Persistence.Outbox.OutboxMessage;

namespace Social.Infrastructure.Persistence.Repositories;

public class OutboxRepository(UserManagementDbContext context) : IOutboxRepository
{
    /// <summary>
    /// Get unprocessed outbox messages
    /// </summary>
    /// <param name="batchSize">Amount of outbox messages retrieved</param>
    /// <returns>An Array of <see cref="OutboxMessage"/></returns>
    public async Task<Outbox_OutboxMessage[]> GetUnprocessedAsync(int batchSize)
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
    public Task MarkAsProcessedAsync(Outbox_OutboxMessage message)
    {
        message.ProcessedOn = DateTime.UtcNow;
        return Task.CompletedTask;
    }
}