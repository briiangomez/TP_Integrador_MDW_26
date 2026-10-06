using System.Diagnostics;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SeguridadApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "postgres",
        tags: new[] { "ready" });

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Remove("traceId");

        if (ctx.ProblemDetails.Status == StatusCodes.Status400BadRequest
            && !ctx.ProblemDetails.Extensions.ContainsKey("errors"))
        {
            ctx.ProblemDetails.Extensions["errors"] = new[] { "El body no es un JSON válido o tiene campos con un tipo incorrecto." };
        }
    };
});

builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = false);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "SeguridadApi - Grupo 7", Version = "v1" });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Trace-Id"] = Activity.Current?.Id ?? context.TraceIdentifier;
        return Task.CompletedTask;
    });
    await next();
});

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapAuthEndpoints();

app.Run();
