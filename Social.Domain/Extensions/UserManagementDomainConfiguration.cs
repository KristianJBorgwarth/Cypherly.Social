using Microsoft.Extensions.DependencyInjection;
using Social.Domain.Interfaces;
using Social.Domain.Services;

namespace Social.Domain.Extensions;

public static class UserManagementDomainConfiguration
{
    public static void AddDomain(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserProfileLifecycleService, UserProfileLifecycleLifecycleService>();
        serviceCollection.AddScoped<IFriendshipService, FriendshipService>();
        serviceCollection.AddScoped<IUserBlockingService, UserBlockingService>();
    }
}