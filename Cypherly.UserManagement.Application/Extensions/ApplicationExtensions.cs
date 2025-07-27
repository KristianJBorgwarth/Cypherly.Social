using System.Reflection;
using Cypherly.UserManagement.Application.Behavior;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.UserManagement.Application.Extensions;

public static class ApplicationExtensions
{
    public static void AddUserManagementApplication(this IServiceCollection services, Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddAutoMapper(assembly);
    }
}