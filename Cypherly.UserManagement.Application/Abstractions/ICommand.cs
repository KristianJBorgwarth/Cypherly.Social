using Cypherly.UserManagement.Domain.Common;
using MediatR;

namespace Cypherly.UserManagement.Application.Abstractions;

public interface ICommand : IRequest<Result> { }
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }

public interface ICommandId : ICommand
{
    public Guid Id { get; init; }
}

public interface ICommandId<TResponse> : ICommand<TResponse>
{
    public Guid Id { get; init; }
}