using System.Reflection;
using System.Text;
using Social.Application.Extensions;
using Cypherly.UserManagement.Domain.Extensions;
using Social.Infrastructure.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Social.API.Extensions;
using Social.API.Filters;

// ReSharper disable UseCollectionExpression

var builder = WebApplication.CreateBuilder(args);

#region Extensions

var env = builder.Environment;

var configuration = builder.Configuration;
configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddEnvironmentVariables();

if (env.IsDevelopment())
{
    configuration.AddJsonFile($"appsettings.{Environments.Development}.json", true, true);
    configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
}

#endregion

#region Logger

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddObservability(configuration);

#endregion

#region Domain Layer

builder.Services.AddUserManagementDomainServices();

#endregion

#region Application Layer

builder.Services.AddApplication(Assembly.Load("Social.Application"));

#endregion

#region Infrastructure Layer

builder.Services.AddInfrastructure(configuration, Assembly.Load("Social.Infrastructure"));

#endregion

#region Authentication & Authorization

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"] ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {configuration["Jwt:Issuer"]}"),
            ValidAudience = configuration["Jwt:Audience"] ?? throw new NotImplementedException($"MISSING VALUE IN JWT SETTINGS {configuration["Jwt:Audience"]}"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"] ??
                                                                               throw new NotImplementedException("MISSING VALUE IN JWT SETTINGS Jwt:Secret")))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IValidateUserIdFilter, ValidateUserIdIdFilter>();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowElectron", policy =>
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
        policy.WithOrigins(allowedOrigins!)
            .AllowAnyMethod()
            .AllowCredentials()
            .AllowAnyHeader();
    });
});

#endregion


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Cypherly.UserManagement.API",
        Version = "v1",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });

    c.AddSecurityRequirement(new()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
    });
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPrometheusScrapingEndpoint();
app.UseSerilogRequestLogging();

if (env.IsProduction())
{
    app.Services.ApplyPendingMigrations();
}

app.UseHttpsRedirection();

app.UseCors("AllowElectron");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

Log.Information("Starting Cypherly.Usermanagament.API"); 
app.Run();

public partial class Program {}