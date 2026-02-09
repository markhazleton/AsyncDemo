# ?? API Documentation with Scalar

AsyncSpark.Web features beautiful, modern API documentation powered by **[Scalar](https://github.com/scalar/scalar)**.

## ?? Quick Start

After running the application, access your API documentation:

### Interactive API Documentation
```
https://localhost:{port}/scalar/v1
```

### OpenAPI Specification
```
https://localhost:{port}/openapi/v1.json
```

### Live Demo
Visit the live demo at: [https://AsyncSpark.azurewebsites.net/scalar/v1](https://AsyncSpark.azurewebsites.net/scalar/v1)

## ? Features

### Beautiful Modern UI
- **Clean Design**: Modern, intuitive interface
- **Dark/Light Mode**: Automatic theme switching
- **Three-Panel Layout**: Navigation, content, and examples
- **Fast & Responsive**: Optimized performance

### Interactive Testing
- **Try It Out**: Test endpoints directly in the browser
- **Request Editor**: Customize headers, query params, and body
- **Real-Time Responses**: See actual API responses
- **Authentication Support**: Test secured endpoints

### Code Generation
Scalar automatically generates code examples in multiple languages:
- **C#** (HttpClient)
- **JavaScript** (fetch, axios)
- **Python** (requests)
- **cURL**
- **PowerShell**
- **Go**
- **Ruby**
- **PHP**
- And more!

### Developer Experience
- **Advanced Search**: Quickly find endpoints
- **Deep Linking**: Share direct links to specific endpoints
- **Keyboard Shortcuts**: Navigate efficiently
- **Copy-Paste Friendly**: Easy code snippet copying
- **Mobile Responsive**: Works great on all devices

## ?? Customization

The API documentation can be customized in `AsyncSpark.Web\Extensions\CustomScalarExtensions.cs`:

### Change Theme
```csharp
options.Theme = ScalarTheme.Purple;  // Available: Default, Moon, Saturn, DeepSpace, etc.
```

### Toggle Features
```csharp
options.DarkMode = true;              // Force dark mode
options.ShowSidebar = false;          // Hide sidebar
options.DefaultHttpClient = new(
    ScalarTarget.JavaScript, 
    ScalarClient.Fetch
);
```

### Custom Branding
```csharp
options.Title = "My API Documentation";
options.Favicon = "/favicon.ico";
```

## ?? Technology Stack

- **Microsoft.AspNetCore.OpenApi** (v10.0.0) - Built-in .NET OpenAPI support
- **Scalar.AspNetCore** (v2.11.0) - Modern API documentation UI
- **.NET 10** - Latest framework features

## ?? Benefits

### Official Microsoft Support
- Part of ASP.NET Core framework
- Long-term support guaranteed
- Regular updates and improvements

### Modern Standards
- OpenAPI 3.0/3.1 compliant
- Industry-standard specification
- Wide tooling compatibility

### Minimal Dependencies
- Lightweight package
- Fast startup time
- Reduced complexity

### Better Developer Experience
- Superior UI compared to traditional tools
- Multiple code language examples
- Intuitive navigation and search

## ?? XML Documentation

This project generates XML documentation automatically. All your code comments (///) appear in the API documentation.

To add or update documentation:

```csharp
/// <summary>
/// Gets weather data for a specific city
/// </summary>
/// <param name="city">The city name</param>
/// <returns>Weather information</returns>
[HttpGet("{city}")]
public async Task<ActionResult<WeatherData>> GetWeather(string city)
{
    // Your code here
}
```

## ??? Advanced Features

### Multiple API Versions
Support for multiple API versions with separate documentation:
```
/scalar/v1
/scalar/v2
```

### Custom Endpoints
Configure custom paths for both OpenAPI spec and Scalar UI:
```csharp
app.MapOpenApi("/api-docs/v1.json");
app.MapScalarApiReference("/docs/v1");
```

### Security Schemes
Document authentication requirements:
- Bearer tokens
- API keys
- OAuth2 flows
- Custom authentication

## ?? Resources

- [Scalar Documentation](https://github.com/scalar/scalar)
- [Scalar NuGet Package](https://www.nuget.org/packages/Scalar.AspNetCore)
- [Microsoft OpenAPI Documentation](https://learn.microsoft.com/aspnet/core/fundamentals/openapi)
- [OpenAPI Specification](https://swagger.io/specification/)

## ?? Best Practices

1. **Write Good XML Comments**: Clear, concise descriptions help users
2. **Use Meaningful Names**: Descriptive endpoint and parameter names
3. **Include Examples**: Add example values for complex types
4. **Document Errors**: Specify possible error responses
5. **Keep It Updated**: Documentation should match your API

## ?? Troubleshooting

### Documentation Not Showing
- Verify the application is running
- Check that `/openapi/v1.json` returns valid JSON
- Ensure `MapOpenApi()` and `MapScalarApiReference()` are called

### XML Comments Not Appearing
- Confirm `<GenerateDocumentationFile>` is set to `True` in .csproj
- Check that XML file is generated in the output directory
- Rebuild the project

### Endpoints Missing
- Ensure controllers have `[ApiController]` attribute
- Verify routes are properly defined
- Check that `AddControllers()` is called in Program.cs

## ?? What Makes Scalar Great

1. **Modern Design**: Beautiful UI that developers love
2. **Fast Performance**: Quick load times and smooth interactions
3. **Code Examples**: Instant code generation in your language
4. **Great UX**: Intuitive navigation and powerful search
5. **Active Development**: Regular updates and new features

---

**Built with ?? on .NET 10 | Powered by Scalar**

For migration documentation and history, see `/docs/migrations/`
