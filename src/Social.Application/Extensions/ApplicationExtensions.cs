using System.Reflection;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Social.Application.Behavior;

namespace Social.Application.Extensions;

public static class ApplicationExtensions
{
    public static void AddApplication(this IServiceCollection services, Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly);
        // Options are read by the source generator at compile time, so they must stay inline constants
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [typeof(ValidationBehavior<,>), typeof(ExceptionBehavior<,>)];
        });
    }
}
