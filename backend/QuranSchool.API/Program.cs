using QuranSchool.Infrastructure;
using QuranSchool.Application;
using Scalar.AspNetCore;
using QuranSchool.Infrastructure.Persistence;
using QuranSchool.Application.Abstractions.Authentication;

using QuranSchool.API.Middleware;

using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 10;
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Quran School API";
        document.Info.Version = "v1";
        document.Info.Description = "API for managing Quran School academic activities.";
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DbInitializer.SeedAsync(context, hasher);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseRateLimiter();
app.MapOpenApi();
app.MapScalarApiReference(options => 
{
    options.Title = "Quran School API";
    options.DefaultHttpClient = new (ScalarTarget.CSharp, ScalarClient.HttpClient);
    options.CustomCss="";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
