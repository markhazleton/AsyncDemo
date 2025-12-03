using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;

namespace AsyncDemo.Web.Extensions;

/// <summary>
/// Custom OpenAPI configuration using built-in .NET OpenAPI + Scalar UI
/// </summary>
public static class CustomScalarExtensions
{
    /// <summary>
    /// Adds OpenAPI documentation generation
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomScalar(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "AsyncDemo.Web API";
                document.Info.Version = "v1";
                document.Info.Description = """
                    <div style="padding: 16px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); border-radius: 8px; margin-bottom: 20px;">
                        <a href="/" style="display: inline-flex; align-items: center; gap: 8px; padding: 10px 20px; background: white; color: #667eea; text-decoration: none; border-radius: 6px; font-weight: 600; font-size: 14px; box-shadow: 0 2px 8px rgba(0,0,0,0.2); transition: all 0.2s;">
                            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" fill="currentColor" viewBox="0 0 16 16">
                                <path d="M8.354 1.146a.5.5 0 0 0-.708 0l-6 6A.5.5 0 0 0 1.5 7.5v7a.5.5 0 0 0 .5.5h4.5a.5.5 0 0 0 .5-.5v-4h2v4a.5.5 0 0 0 .5.5H14a.5.5 0 0 0 .5-.5v-7a.5.5 0 0 0-.146-.354L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.354 1.146zM2.5 14V7.707l5.5-5.5 5.5 5.5V14H10v-4a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 0-.5.5v4H2.5z"/>
                            </svg>
                            ← Back to Home
                        </a>
                    </div>
                    
                    <p style="margin: 16px 0; font-size: 16px; line-height: 1.6;">
                        AsyncDemo.Web API built with ASP.NET to show how to create RESTful services using a decoupled, maintainable architecture.
                    </p>
                    
                    <p style="margin: 16px 0; font-size: 14px; color: #666;">
                        Explore asynchronous programming techniques, resilience patterns with Polly, and real-world API integrations.
                    </p>
                    """;
                
                document.Info.Contact = new()
                {
                    Name = "Mark Hazleton",
                    Email = "mark@markhazleton.com",
                    Url = new Uri("https://markhazleton.com/")
                };
                
                document.Info.License = new()
                {
                    Name = "MIT"
                };
                
                return Task.CompletedTask;
            });
        });
        
        return services;
    }

    /// <summary>
    /// Configures the application to use OpenAPI and Scalar UI
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application with Scalar configured</returns>
    public static WebApplication UseCustomScalar(this WebApplication app)
    {
        // Serves the OpenAPI spec at /openapi/v1.json
        app.MapOpenApi();
        
        // Serves the beautiful Scalar UI at /scalar/v1
        app.MapScalarApiReference(options =>
        {
            options.Title = "AsyncDemo.Web API";
            options.Theme = ScalarTheme.Default;
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.ShowSidebar = true;
        });
        
        return app;
    }
}
