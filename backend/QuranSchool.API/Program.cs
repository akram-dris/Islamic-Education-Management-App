using QuranSchool.Infrastructure;
using QuranSchool.Application;
using Scalar.AspNetCore;
using QuranSchool.Infrastructure.Persistence;
using QuranSchool.Application.Abstractions.Authentication;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

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
