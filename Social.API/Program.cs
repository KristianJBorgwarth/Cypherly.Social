using System.Reflection;
using Social.Application.Extensions;
using Social.Infrastructure.Extensions;
using Scalar.AspNetCore;
using Social.API.Extensions;
using Social.Domain.Extensions;

// ReSharper disable UseCollectionExpression


var builder = WebApplication.CreateBuilder(args);

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

builder.AddLogging();
builder.Services.AddObservability(configuration);

builder.Services.AddDomain();

builder.Services.AddApplication(Assembly.Load("Social.Application"));

builder.Services.AddInfrastructure(configuration, Assembly.Load("Social.Infrastructure"));

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

builder.Services.AddControllers();
builder.Services.AddOpenApi();


var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Social.API V1")
        .WithTheme(ScalarTheme.Purple)
        .HideDarkModeToggle()
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
});

if (env.IsProduction())
{
    app.Services.ApplyPendingMigrations();
}

app.UseHttpsRedirection();

app.UseCors("AllowElectron");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program {}
