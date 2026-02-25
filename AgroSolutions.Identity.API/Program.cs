using AgroSolutions.Identity.API.Filters;
using AgroSolutions.Identity.API.Middleware;
using AgroSolutions.Identity.Application;
using AgroSolutions.Identity.Infrastructure;
using AgroSolutions.Identity.Infrastructure.Messaging;
using AgroSolutions.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] (CorrelationId={CorrelationId}) {Message:lj} {NewLine}{Exception}")
    .WriteTo.GrafanaLoki("http://loki:3100", [
        new()
        {
            Key = "app",
            Value = "agro-solution-identity-api"
        }
    ])
    .CreateLogger();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services
    .AddControllers(options => options.Filters.Add<RestResponseFilter>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "AgroSolutions - Identity",
        Description = "Developed by Mário Guilherme de Andrade Rodrigues",
        Version = "v1",
        Contact = new()
        {
            Name = "Mário Guilherme de Andrade Rodrigues",
            Email = "marioguilhermedev@gmail.com"
        },
        License = new()
        {
            Name = "MIT",
            Url = new("https://opensource.org/licenses/MIT")
        }
    });

    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
    });
});
builder.Services.AddOpenApi();

builder.Services.AddOpenTelemetry()
    .WithTracing(tpb =>
    {
        tpb
            .AddSource("AgroSolutions")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("AgroSolutions")
            .AddAttributes(new Dictionary<string, object>
            {
                ["service.namespace"] = "AgroSolutions",
                ["deployment.environment"] = builder.Configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development"
            }))
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.Filter = httpContext => !httpContext.Request.Path.StartsWithSegments("/health");
            })
            .AddSqlClientInstrumentation(options => options.RecordException = true);

        //if (builder.Configuration["Observability:UseOtlp"]?.ToLower() == "true")
        {
            string otlpEndpoint = /*builder.Configuration["Observability:OtlpEndpoint"] ??*/ "http://grafana:3000";
            tpb.AddOtlpExporter(options =>
            {
                options.Endpoint = new(otlpEndpoint);
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
        }

        if (builder.Configuration["ASPNETCORE_ENVIRONMENT"] == "Development") tpb.AddConsoleExporter();
    });

WebApplication app = builder.Build();

using AsyncServiceScope asyncServiceScope = app.Services.CreateAsyncScope();
IServiceProvider services = asyncServiceScope.ServiceProvider;

#region Ensures the database is created at startup.
try
{
    AgroSolutionsIdentityDbContext context = services.GetRequiredService<AgroSolutionsIdentityDbContext>();
    if ((await context.Database.GetPendingMigrationsAsync()).Any())
        await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error during database initialization.");
}
#endregion

#region Ensures the creation of the exchange, queues, and message binds at startup.
try
{
    IMessagingConnectionFactory factory = services.GetRequiredService<IMessagingConnectionFactory>();
    IOptions<RabbitMqOptions> options = services.GetRequiredService<IOptions<RabbitMqOptions>>();
    await RabbitMqConnection.InitializeAsync(await factory.CreateChannelAsync(CancellationToken.None), options.Value);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Error during messaging initialization.");
}
#endregion

app.UseSerilogRequestLogging();
app.UseMetricServer();
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapMetrics();

app.MapHealthChecks("/health");

await app.RunAsync();

Log.CloseAndFlush();
