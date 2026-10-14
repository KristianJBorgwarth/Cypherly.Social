using Social.Domain.Common;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.Logging;

// ReSharper disable InvertIf

namespace Social.Application.Behavior
{
    public class ValidationBehavior<TRequest, TResponse>(
        ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        IValidator<TRequest>? validator = null)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IMessage
        where TResponse : Result
    {
        public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
        {
            if (validator is null)
                return await next(request, cancellationToken);

            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid)
                return await next(request, cancellationToken);

            var errorMessage = string.Join("; ", validationResult.Errors.Select(e => $"{e.ErrorMessage} ({e.PropertyName})"));

            logger.LogWarning("Validation failed for {RequestType}: {ErrorMessage}", typeof(TRequest).Name, errorMessage);

            var error = Error.Validation("Validation failed: " + errorMessage);

            return typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>)
                ? CreateGenericFailResponse(error)
                : CreateFailResponse(error);
        }

        private static TResponse CreateFailResponse(Error error)
        {
            return (TResponse)Result.Fail(error);
        }

        private static TResponse CreateGenericFailResponse(Error error)
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var method = typeof(Result).GetMethods()
                .FirstOrDefault(m => m is { Name: "Fail", IsGenericMethodDefinition: true })!;

            var genericFailMethod = method.MakeGenericMethod(resultType);
            return (TResponse)genericFailMethod.Invoke(null, [error])!;
        }
    }
}
