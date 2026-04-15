using Asp.Versioning;
using Brotherhood_Portal.API.Extensions;
using Brotherhood_Portal.API.GraphQL.Schema;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Extension Methods for Service Registration
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddJwtAuthentication(builder.Configuration)
    .AddAppIdentity()
    .AddApiCors(builder.Configuration)
    .AddApiRateLimiting();

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy =
        System.Text.Json.JsonNamingPolicy.CamelCase;
});

// GraphQL Services
builder.Services.AddGraphQLSchema();
builder.Services.AddAuthorization();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDBContext>()
    .AddCheck("self", () => HealthCheckResult.Healthy());

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequirePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Correlation ID
app.Use(async (context, next) =>
{
    const string headerName = "X-Correlation-ID";

    // Check if the incoming request has a correlation ID header. If not, generate a new one.
    if (!context.Request.Headers.TryGetValue(headerName, out var correlationId))
    {
        correlationId = Guid.NewGuid().ToString();
        context.Request.Headers[headerName] = correlationId;
    }

    // Attach the correlation ID to the response headers so that clients can correlate requests and responses.
    context.Response.Headers[headerName] = correlationId;

    // Store the correlation ID in the HttpContext.Items collection so that it can be accessed throughout the request processing pipeline.
    context.Items["CorrelationId"] = correlationId.ToString();

    await next();
});

// Log request Automatically
app.UseSerilogRequestLogging(options =>
{
    // Decide log severity
    options.GetLevel = (httpContext, elapsed, ex) =>
    {
        if (ex != null || httpContext.Response.StatusCode >= 500)
            return Serilog.Events.LogEventLevel.Error;

        if (elapsed > 2000) // elapsed is already milliseconds
            return Serilog.Events.LogEventLevel.Warning;

        return Serilog.Events.LogEventLevel.Information;
    };

    // Enrich log with additional context
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        var correlationId = httpContext.Items["CorrelationId"]?.ToString() ?? httpContext.TraceIdentifier;

        diagnosticContext.Set("CorrelationId", correlationId);

        diagnosticContext.Set("RequestHost", string.IsNullOrWhiteSpace(httpContext.Request.Host.Value)
            ? "unknown-host" : httpContext.Request.Host.Value);

        diagnosticContext.Set( "RequestScheme", string.IsNullOrWhiteSpace(httpContext.Request.Scheme)
            ? "unknown-scheme" : httpContext.Request.Scheme);

        diagnosticContext.Set("RequestPath", string.IsNullOrWhiteSpace(httpContext.Request.Path) 
            ? "unknown-path" : httpContext.Request.Path);

        diagnosticContext.Set("RequestMethod", string.IsNullOrWhiteSpace(httpContext.Request.Method) 
            ? "unknown-method" : httpContext.Request.Method);

        diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip");

        diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString() ?? "unknown-agent");

        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            diagnosticContext.Set("UserId", httpContext.User.Identity.Name ?? "unknown-user");
        }
    };
});

app.MapHealthChecks("/health");

// if (app.Environment.IsDevelopment())
// {
//    app.UseSwagger();
//    app.UseSwaggerUI();
// }

var enableSwagger = builder.Configuration.GetValue<bool>("EnableSwagger");

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Security
app.UseSecurity(app.Environment);

// CQRS
app.UseCors("CorsPolicy");

// Rate Limiting
app.UseRateLimiter();

// Authentication
app.UseAuthentication(); //Who are you?

//Authorization
app.UseAuthorization(); //Are you allowed?

// Controllers
app.MapControllers().RequireRateLimiting("fixed");

// GraphQl
app.MapGraphQL("/graphql"); // GraphQL endpoint

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

    try
    {
        Log.Information("Applying database migrations...");
        db.Database.Migrate();
        Log.Information("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Database migration failed");
        throw;
    }
}

// Run
app.Run();
