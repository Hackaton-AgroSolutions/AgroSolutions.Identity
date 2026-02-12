using Serilog;
using Serilog.Context;

namespace AgroSolutions.Identity.API.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        string correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            Log.Information("Start of request with CorrelationId {CorrelationId}.", correlationId);
            await _next(context);
        }
    }
}
