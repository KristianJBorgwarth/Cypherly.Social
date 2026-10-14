using Social.Domain.Common;
using Mediator;

namespace Social.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }

public interface IQueryLimited<TResponse> : IRequest<Result<TResponse>>
{
    public Guid TenantId { get; init; }
}