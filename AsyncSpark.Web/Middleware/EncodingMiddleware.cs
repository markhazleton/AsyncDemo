namespace AsyncSpark.Web.Middleware;

/// <summary>
/// Middleware to ensure proper UTF-8 encoding for all responses
/// </summary>
public class EncodingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<EncodingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EncodingMiddleware"/> class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger instance</param>
    public EncodingMiddleware(RequestDelegate next, ILogger<EncodingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes an HTTP request and ensures UTF-8 encoding
    /// </summary>
    /// <param name="context">The HTTP context for the current request</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        // Set request encoding
        context.Request.ContentType = context.Request.ContentType?.Replace("charset=", "charset=utf-8") ?? "text/html; charset=utf-8";

        // Store original response stream
        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // Ensure response has UTF-8 charset
            if (context.Response.ContentType != null && !context.Response.ContentType.Contains("charset="))
            {
                context.Response.ContentType += "; charset=utf-8";
            }
            else if (context.Response.ContentType == null)
            {
                context.Response.ContentType = "text/html; charset=utf-8";
            }

            // Copy response back to original stream
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in encoding middleware");
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}

/// <summary>
/// Extension methods for registering the encoding middleware
/// </summary>
public static class EncodingMiddlewareExtensions
{
    /// <summary>
    /// Adds encoding middleware to the pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder</returns>
    public static IApplicationBuilder UseEncodingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<EncodingMiddleware>();
    }
}
