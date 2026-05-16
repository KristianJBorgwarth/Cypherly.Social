using Microsoft.IdentityModel.Tokens;
using Social.API.Extensions;

namespace Social.API.Extensions;

internal static class AuthenticationExtensions
{
    public static void AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var authority = configuration["Jwt:Authority"]
            ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {configuration["Jwt:Authority"]}");

        var audience = configuration["Jwt:Audience"]
            ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {configuration["Jwt:Audience"]}");

        services.AddAuthentication().AddJwtBearer(options =>
        {
            options.Authority = authority;
            options.Audience = audience;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = authority,
                ValidAudience = audience,
            };
        });

        services.AddAuthorization();
    }
}
