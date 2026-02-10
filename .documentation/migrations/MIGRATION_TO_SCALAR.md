# Using Built-in .NET OpenAPI with Scalar UI

## Overview

This approach uses .NET's built-in OpenAPI support (introduced in .NET 9) combined with Scalar for a modern, beautiful documentation UI.

## Benefits

? **No Third-Party API Generation Library**
- Uses Microsoft's official OpenAPI implementation
- Minimal external dependencies
- Long-term support guaranteed

? **Modern, Beautiful UI**
- Scalar provides a much better UI than Swagger UI
- Fast and responsive
- Great developer experience

? **Lightweight**
- Smaller package size
- Faster startup
- Less complexity

## Setup for AsyncSpark.Web

### Step 1: Update Package References

**Remove:**
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
```

**Add:**
```xml
<PackageReference Include="Scalar.AspNetCore" Version="1.2.42" />
```

### Step 2: Update CustomSwaggerExtensions.cs

Replace with this simplified version:

```csharp
namespace AsyncSpark.Web.Extensions;

/// <summary>
/// Custom OpenAPI configuration using built-in .NET OpenAPI + Scalar UI
/// </summary>
public static class CustomSwaggerExtensions
{
    /// <summary>
    /// Adds OpenAPI documentation generation
    /// </summary>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "AsyncSpark.Web API";
                document.Info.Version = "v1";
                document.Info.Description = "AsyncSpark.Web API built with ASP.NET to show how to create RESTful services using a decoupled, maintainable architecture.";
                
                document.Info.Contact = new()
                {
                    Name = "Mark Hazleton",
                    Email = "mark.hazleton@controlorigins.com",
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
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
    {
        // Serves the OpenAPI spec at /openapi/v1.json
        app.MapOpenApi();
        
        // Serves the beautiful Scalar UI at /scalar/v1
        app.MapScalarApiReference(options =>
        {
            options.Title = "AsyncSpark.Web API";
            options.Theme = ScalarTheme.Default;
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.ShowSidebar = true;
        });
        
        return app;
    }
}
```

### Step 3: Update Program.cs (Optional)

Your existing code should work, but you can also use this cleaner approach:

```csharp
// Registration
builder.Services.AddCustomSwagger();

// Middleware (after app is built)
app.UseCustomSwagger();

// Or use directly without extension methods:
app.MapOpenApi();
app.MapScalarApiReference();
```

### Step 4: Access Documentation

- **Scalar UI**: `https://localhost:{port}/scalar/v1`
- **OpenAPI Spec**: `https://localhost:{port}/openapi/v1.json`

## Scalar UI Features

### Beautiful Modern Interface
- Clean, modern design
- Three-panel layout (navigation, content, examples)
- Dark/light mode support
- Fast and responsive

### Interactive API Testing
- Built-in request editor
- Code generation in multiple languages:
  - C# (HttpClient)
  - JavaScript (fetch, axios)
  - Python
  - cURL
  - And more!

### Developer-Friendly
- Search functionality
- Keyboard shortcuts
- Copy-paste friendly
- Deep linking to specific endpoints

## Comparison: Scalar vs Swagger UI

| Feature | Swagger UI | Scalar |
|---------|------------|--------|
| Design | Classic | Modern |
| Performance | Good | Excellent |
| Code Examples | Limited | Multiple languages |
| Dark Mode | Via theme | Built-in |
| Search | Basic | Advanced |
| UX | Traditional | Modern |

## XML Comments Support

To include XML documentation comments:

```csharp
services.AddOpenApi(options =>
{
    var xmlFile = "AsyncSpark.Web.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", xmlFile);
    if (!File.Exists(xmlPath))
    {
        xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    }
    
    // Built-in OpenAPI automatically includes XML comments
    // from the <DocumentationFile> specified in .csproj
});
```

Your project already has this configured:
```xml
<GenerateDocumentationFile>True</GenerateDocumentationFile>
<DocumentationFile>wwwroot\AsyncSpark.Web.xml</DocumentationFile>
```

## Advanced Configuration

### Custom Themes

```csharp
app.MapScalarApiReference(options =>
{
    options.Theme = ScalarTheme.Purple; // or Default, BluePlanet, Saturn, etc.
    options.DarkMode = true;
});
```

### Multiple API Versions

```csharp
app.MapOpenApi("/openapi/v1.json").WithName("v1");
app.MapOpenApi("/openapi/v2.json").WithName("v2");

app.MapScalarApiReference(options =>
{
    options.EndpointPathPrefix = "/scalar/{documentName}";
});
```

### Security Schemes

```csharp
services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["Bearer"] = new()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token"
            }
        };
        return Task.CompletedTask;
    });
});
```

## Advantages

1. **Official Microsoft Support**
   - Part of ASP.NET Core
   - Long-term support
   - Well-maintained

2. **Modern UI**
   - Better UX than Swagger UI
   - More intuitive
   - Better for public APIs

3. **Minimal Dependencies**
   - Only Scalar package needed
   - Smaller footprint
   - Faster startup

4. **Future-Proof**
   - OpenAPI 3.1 support
   - Regular updates
   - Modern standards

## Limitations

1. **Less Customization** (compared to Swashbuckle/NSwag)
   - Fewer hooks and processors
   - Limited schema customization
   - Simpler configuration

2. **Newer Technology**
   - Smaller community
   - Less Stack Overflow content
   - Fewer examples

3. **No Client Generation**
   - Scalar is UI-only
   - Need separate tool for client generation

## Migration Checklist

- [ ] Update package reference to Scalar.AspNetCore
- [ ] Remove Swashbuckle.AspNetCore package
- [ ] Update CustomSwaggerExtensions.cs
- [ ] Test build
- [ ] Test Scalar UI at /scalar/v1
- [ ] Verify OpenAPI spec at /openapi/v1.json
- [ ] Check all endpoints are documented
- [ ] Test "Try it out" functionality
- [ ] Verify XML comments appear

## Resources

- [Scalar Documentation](https://github.com/scalar/scalar)
- [Scalar.AspNetCore NuGet](https://www.nuget.org/packages/Scalar.AspNetCore)
- [Microsoft OpenAPI Docs](https://learn.microsoft.com/aspnet/core/fundamentals/openapi)
- [OpenAPI Specification](https://swagger.io/specification/)

## Rollback

If needed, simply:
1. Revert package changes
2. Restore original CustomSwaggerExtensions.cs
3. Run `dotnet restore`
