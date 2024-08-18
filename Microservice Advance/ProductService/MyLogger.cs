namespace ProductService;

public class MyLogger(RequestDelegate next, ILogger<MyLogger> logger)
{
    private readonly ILogger _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        var traceId = context.Request.Headers["X-Trace-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        context.Request.Headers["X-Trace-ID"] = traceId;

        var watch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Request {TraceId} started: {Method} {Path}", 
                traceId, context.Request.Method, context.Request.Path);

            await next(context);
        }
        finally
        {
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            
            _logger.LogInformation("Request {TraceId} completed: {Method} {Path} - Status: {StatusCode} - Took: {ElapsedMs} ms",
                traceId, context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsedMs);
        }
    }
}