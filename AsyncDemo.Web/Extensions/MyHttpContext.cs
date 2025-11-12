namespace AsyncDemo.Web.Extensions;

/// <summary>
/// 
/// </summary>
public class MyHttpContext
{
    private static IHttpContextAccessor m_httpContextAccessor;

    /// <summary>
    /// Gets the current HttpContext
    /// </summary>
    public static HttpContext? Current => m_httpContextAccessor?.HttpContext;

    /// <summary>
    /// Gets the application base URL
    /// </summary>
    public static string? AppBaseUrl => Current != null ? $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}" : null;

    internal static void Configure(IHttpContextAccessor contextAccessor)
    { m_httpContextAccessor = contextAccessor; }
}
