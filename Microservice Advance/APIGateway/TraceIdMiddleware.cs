namespace APIGateway;

public class TraceIdMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-Trace-ID", out var traceId))
        {
            context.Request.Headers.Append("X-Trace-ID", Guid.NewGuid().ToString());
        }

        await next(context);
    }
}