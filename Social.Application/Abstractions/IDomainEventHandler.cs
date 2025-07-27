using Cypherly.UserManagement.Domain.Abstractions;
using MediatR;

namespace Cypherly.Application.Abstractions;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{

}