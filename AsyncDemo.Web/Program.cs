using AsyncDemo.HttpGetCall;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Westwind.AspNetCore.Markdown;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks()
    .AddCheck("API Health", () => HealthCheckResult.Healthy("API is healthy"));
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.TryAddSingleton<IMemoryCacheManager, MemoryCacheManager>();
builder.Services.AddCustomSwagger();
builder.Services.AddMarkdown();
builder.Services.AddSession();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddScoped(serviceProvider =>
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
    // Global exception handler for production
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseHttpsRedirection();
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

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("From Program, running the host now.");
app.Run();
