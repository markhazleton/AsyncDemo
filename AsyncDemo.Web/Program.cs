using AsyncDemo.HttpGetCall;
using AsyncDemo.Web.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Westwind.AspNetCore.Markdown;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Configure HttpClient with default timeout and retry policies
builder.Services.AddHttpClient("default", client =>
{
  client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddMemoryCache();

// Register new services
builder.Services.AddScoped<IConfigurationValidationService, ConfigurationValidationService>();
builder.Services.AddScoped<IStartupService, StartupService>();
builder.Services.AddHostedService<StartupHostedService>();

// Enhanced health checks
builder.Services.AddHealthChecks()
    .AddCheck("API Health", () => HealthCheckResult.Healthy("API is healthy"))
    .AddCheck<ConfigurationHealthCheck>("Configuration")
    .AddCheck("Memory Cache", () =>
  {
        try
        {
     var cache = builder.Services.BuildServiceProvider().GetService<IMemoryCache>();
            return cache != null ? HealthCheckResult.Healthy("Memory cache is available") 
          : HealthCheckResult.Unhealthy("Memory cache is not available");
        }
      catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Memory cache check failed: {ex.Message}");
        }
  });

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.TryAddSingleton<IMemoryCacheManager, MemoryCacheManager>();
builder.Services.AddCustomSwagger();
builder.Services.AddMarkdown();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure forwarded headers for reverse proxy scenarios
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

// Register HttpGetCallService with proper DI
builder.Services.AddScoped<IHttpGetCallService>(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<HttpGetCallService>>();
    var telemetryLogger = serviceProvider.GetRequiredService<ILogger<HttpGetCallServiceTelemetry>>();
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
    IHttpGetCallService baseService = new HttpGetCallService(logger, httpClientFactory);
    IHttpGetCallService telemetryService = new HttpGetCallServiceTelemetry(telemetryLogger, baseService);
    return telemetryService;
});

// Register the OpenWeatherMapClient with Decorator Pattern
builder.Services.AddScoped<IOpenWeatherMapClient>(serviceProvider =>
{
  string apiKey = builder.Configuration["OpenWeatherMapApiKey"] ?? "KEYMISSING";
    var logger = serviceProvider.GetRequiredService<ILogger<WeatherServiceClient>>();
    var loggerLogging = serviceProvider.GetRequiredService<ILogger<WeatherServiceLoggingDecorator>>();
    var loggerCaching = serviceProvider.GetRequiredService<ILogger<WeatherServiceCachingDecorator>>();
    var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

  IOpenWeatherMapClient concreteService = new WeatherServiceClient(apiKey, httpClientFactory, logger);
    IOpenWeatherMapClient withLoggingDecorator = new WeatherServiceLoggingDecorator(concreteService, loggerLogging);
    IOpenWeatherMapClient withCachingDecorator = new WeatherServiceCachingDecorator(withLoggingDecorator, memoryCache, loggerCaching);
    return withCachingDecorator;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
else
{
    // Enhanced exception handler for production
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Add HTTP Strict Transport Security
}

// Use forwarded headers for reverse proxy scenarios
app.UseForwardedHeaders();

// Add request logging middleware early in the pipeline
app.UseRequestLogging();

app.UseStaticFiles();
app.UseHttpsRedirection();

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseAuthorization();
app.UseMarkdown();
app.UseSession();

// Add MVC routing first to ensure the default route takes precedence
app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});

// Add Swagger after MVC routing so it doesn't take over the root path
app.UseCustomSwagger();

app.MapControllers();
app.MapHealthChecks("/health");

// Add graceful shutdown logging
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("AsyncDemo.Web application starting up...");

try
{
    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    logger.LogInformation("AsyncDemo.Web application shut down complete");
}
