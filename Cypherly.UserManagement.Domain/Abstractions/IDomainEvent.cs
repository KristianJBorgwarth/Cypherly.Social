using MediatR;

namespace Cypherly.UserManagement.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}