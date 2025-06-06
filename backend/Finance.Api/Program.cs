using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Data;
using DotNetEnv;
using Finance.Api.Helpers;
using Finance.Api.Models;
using Finance.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// Load environment variables from .env file
var envPath = Path.Combine(AppContext.BaseDirectory, "app.env");
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

// add authentication and authorization services
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JWT"));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FinanceDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.User.RequireUniqueEmail = true;
});


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        var jwtIssuer = builder.Configuration["Jwt:Issuer"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<JwtTokenGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🧠 We’ll use this to check the environment
var env = app.Environment;

// 🧩 Middleware to add a correlation ID to each request
app.UseMiddleware<Finance.Api.CorrelationIdMiddleware>();

// ensure controllers are mapped
app.MapControllers();

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

app.UseAuthentication(); // 👈 must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();