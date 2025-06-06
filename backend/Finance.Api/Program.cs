using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Data;
using DotNetEnv;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

// Load environment variables from .env file
var envPath = Path.Combine(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName)!.FullName, ".env");
DotNetEnv.Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);

// Register DbContext and other services
builder.Services.AddDbContext<FinanceDbContext>(options =>
{
    var connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
                           $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};" +
                           $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};" +
                           $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                           $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";
    
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🧠 We’ll use this to check the environment
var env = app.Environment;

// 🧩 Middleware to add a correlation ID to each request
app.UseMiddleware<Finance.Api.CorrelationIdMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

// 🛡 Global exception handler middleware using ProblemDetails (RFC7807)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        // We return JSON for all errors
        context.Response.ContentType = "application/json";

        // Get the exception that was thrown
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        // Logger from DI
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        logger.LogError(exception, "Unhandled exception");

        // Choose appropriate status code based on exception type
        var statusCode = exception switch
        {
            DbException => StatusCodes.Status503ServiceUnavailable,
            ArgumentException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
        
        context.Response.StatusCode = statusCode;
        
        // Retrieve the correlation ID from the request
        var correlationId = context.Items["X-Correlation-ID"]?.ToString();
        
        // Build an RFC7807-compliant ProblemDetails response with optional stack trace
        var problem = new ProblemDetails
        {
            Title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status503ServiceUnavailable => "Service Unavailable",
                _ => "Internal Server Error"
            },
            Status = statusCode,
            Detail = env.IsDevelopment() ? exception?.ToString() : exception?.Message,
            Instance = context.Request.Path,
            Extensions = { ["correlationId"] = correlationId }
        };

        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.UseAuthorization();

app.MapControllers();

app.Run();