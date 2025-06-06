namespace Finance.Api;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    
    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Use existing header or generate new one
        if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers[HeaderName] = correlationId;
        }

        // Store in HttpContext.Items so others can access it
        context.Items[HeaderName] = correlationId;

        // Add to response header for frontend access
        context.Response.Headers[HeaderName] = correlationId;

        // Add to logging scope for structured logs
        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   [HeaderName] = correlationId.ToString()
               }))
        {
            await _next(context);
        }
    }
}