# Migration from Swashbuckle to NSwag

## Why NSwag?

NSwag is the most feature-complete alternative to Swashbuckle and doesn't have the v10 breaking change issues.

## Migration Steps for AsyncDemo.Web

### Step 1: Update Package Reference

**Remove:**
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
```

**Add:**
```xml
<PackageReference Include="NSwag.AspNetCore" Version="14.0.3" />
```

### Step 2: Update CustomSwaggerExtensions.cs

Replace the entire file with:

```csharp
using NSwag;
using NSwag.Generation.Processors.Security;

namespace AsyncDemo.Web.Extensions;

/// <summary>
/// Custom OpenAPI/Swagger configuration using NSwag
/// </summary>
public static class CustomSwaggerExtensions
{
    /// <summary>
    /// Adds NSwag OpenAPI documentation generation
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddOpenApiDocument(config =>
        {
            config.Title = "AsyncDemo.Web API";
            config.Version = "v1";
            config.Description = "AsyncDemo.Web API built with ASP.NET to show how to create RESTful services using a decoupled, maintainable architecture. <br/><a href='/'>Back To Home</a>";
            
            config.PostProcess = document =>
            {
                document.Info.Contact = new OpenApiContact
                {
                    Name = "Mark Hazleton",
                    Email = "mark.hazleton@controlorigins.com",
                    Url = "https://markhazleton.com/"
                };
                
                document.Info.License = new OpenApiLicense
                {
                    Name = "MIT"
                };
            };

            // Include XML comments
            var xmlFile = "AsyncDemo.Web.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", xmlFile);
            if (!File.Exists(xmlPath))
            {
                xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            }
            
            if (File.Exists(xmlPath))
            {
                config.DocumentProcessors.Add(
                    new NSwag.Generation.Processors.DocumentProcessor(xmlPath));
            }
        });
        
        return services;
    }

    /// <summary>
    /// Configures the application to use NSwag OpenAPI and Swagger UI
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder with NSwag configured</returns>
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
    {
        // Serves the OpenAPI spec at /swagger/v1/swagger.json
        app.UseOpenApi(config =>
        {
            config.Path = "/swagger/v1/swagger.json";
        });
        
        // Serves the Swagger UI at /swagger
        app.UseSwaggerUi(config =>
        {
            config.Path = "/swagger";
            config.DocumentPath = "/swagger/v1/swagger.json";
            config.DocumentTitle = "API";
        });
        
        return app;
    }
}
```

### Step 3: No Changes Needed to Program.cs

Your existing code should work as-is:

```csharp
// Registration
builder.Services.AddCustomSwagger();

// Middleware
app.UseCustomSwagger();
```

### Step 4: Test the Migration

1. **Build the project:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run --project AsyncDemo.Web
   ```

3. **Verify Swagger UI:**
   - Navigate to: `https://localhost:{port}/swagger`
   - Check that all endpoints are visible
   - Test the "Try it out" functionality

4. **Verify OpenAPI spec:**
   - Navigate to: `https://localhost:{port}/swagger/v1/swagger.json`
   - Verify JSON is valid

## Key Differences

### API Naming

| Swashbuckle | NSwag |
|-------------|-------|
| `AddSwaggerGen()` | `AddOpenApiDocument()` |
| `UseSwagger()` | `UseOpenApi()` |
| `UseSwaggerUI()` | `UseSwaggerUi()` |
| `Microsoft.OpenApi.Models` | `NSwag` namespace |

### Configuration Style

**Swashbuckle:**
```csharp
services.AddSwaggerGen(cfg =>
{
    cfg.SwaggerDoc("v1", new OpenApiInfo { ... });
});
```

**NSwag:**
```csharp
services.AddOpenApiDocument(config =>
{
    config.Title = "...";
    config.PostProcess = document => { ... };
});
```

## Advantages of NSwag Over Swashbuckle

1. **Better Schema Generation**
   - More accurate type representations
   - Better handling of complex types
   - Improved polymorphism support

2. **Client Code Generation**
   - Generate TypeScript clients
   - Generate C# clients
   - Keep frontend and backend in sync

3. **More Flexible Configuration**
   - Processor pipeline for customization
   - Better control over schema generation
   - Multiple document support

4. **Active Development**
   - Regular updates
   - Modern OpenAPI 3.1 support
   - No v10-style breaking changes

## Additional NSwag Features

### Generate TypeScript Client

```bash
dotnet tool install -g NSwag.ConsoleCore
nswag openapi2tsclient /input:swagger.json /output:api-client.ts
```

### Multiple API Versions

```csharp
services.AddOpenApiDocument(config =>
{
    config.DocumentName = "v1";
    config.Title = "API v1";
});

services.AddOpenApiDocument(config =>
{
    config.DocumentName = "v2";
    config.Title = "API v2";
});
```

### Custom Processors

```csharp
config.OperationProcessors.Add(new MyCustomOperationProcessor());
config.DocumentProcessors.Add(new MyCustomDocumentProcessor());
```

## Rollback Plan

If you need to rollback:

1. Revert package changes
2. Restore original `CustomSwaggerExtensions.cs`
3. Run `dotnet restore`

## Resources

- [NSwag GitHub](https://github.com/RicoSuter/NSwag)
- [NSwag Documentation](https://github.com/RicoSuter/NSwag/wiki)
- [NSwag NuGet](https://www.nuget.org/packages/NSwag.AspNetCore)
