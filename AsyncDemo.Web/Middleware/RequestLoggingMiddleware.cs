namespace AsyncDemo.Web.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
 {
      var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        
        // Add request ID to response headers for tracing
        context.Response.Headers.Add("X-Request-ID", requestId);

        _logger.LogInformation(
            "Request {RequestId} started: {Method} {Path} from {RemoteIpAddress}",
       requestId,
        context.Request.Method,
            context.Request.Path,
context.Connection.RemoteIpAddress);

        try
        {
      await _next(context);
      }
     catch (Exception ex)
     {
      _logger.LogError(ex, 
     "Request {RequestId} failed: {Method} {Path}",
     requestId,
    context.Request.Method,
             context.Request.Path);
       throw;
    }
      finally
      {
    stopwatch.Stop();
            
        var level = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
      
    _logger.Log(level,
   "Request {RequestId} completed: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
     requestId,
    context.Request.Method,
      context.Request.Path,
    context.Response.StatusCode,
     stopwatch.ElapsedMilliseconds);
   }
    }
}

/// <summary>
/// Extension methods for registering the request logging middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds request logging middleware to the pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}