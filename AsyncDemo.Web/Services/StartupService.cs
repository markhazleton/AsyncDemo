namespace AsyncDemo.Web.Services;

/// <summary>
/// Service that runs during application startup to perform initialization tasks
/// </summary>
public interface IStartupService
{
    /// <summary>
    /// Executes startup tasks
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the startup operation</returns>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of startup service
/// </summary>
public class StartupService : IStartupService
{
    private readonly IConfigurationValidationService _configValidationService;
    private readonly ILogger<StartupService> _logger;
    private readonly IMemoryCache _memoryCache;

    public StartupService(
        IConfigurationValidationService configValidationService,
        ILogger<StartupService> logger,
        IMemoryCache memoryCache)
    {
        _configValidationService = configValidationService ?? throw new ArgumentNullException(nameof(configValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting application initialization...");

    try
    {
            // Validate configuration
   var configResult = _configValidationService.ValidateConfiguration();
  if (!configResult.IsValid)
            {
  _logger.LogError("Application startup failed due to configuration errors");
          throw new InvalidOperationException("Configuration validation failed");
            }

        // Pre-warm cache with application status
   var applicationStatus = new ApplicationStatus(Assembly.GetExecutingAssembly());
     _memoryCache.Set("ApplicationStatusCache", applicationStatus, TimeSpan.FromHours(24));

         // Add any other startup tasks here
            await Task.Delay(100, cancellationToken); // Simulate some async startup work

            _logger.LogInformation("Application initialization completed successfully");
        }
        catch (Exception ex)
        {
      _logger.LogError(ex, "Application initialization failed");
   throw;
 }
    }
}

/// <summary>
/// Hosted service that runs the startup service
/// </summary>
public class StartupHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StartupHostedService> _logger;

    public StartupHostedService(IServiceProvider serviceProvider, ILogger<StartupHostedService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
     var startupService = scope.ServiceProvider.GetRequiredService<IStartupService>();
       await startupService.ExecuteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
  _logger.LogError(ex, "Startup service failed");
            throw;
     }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
 _logger.LogInformation("Startup hosted service is stopping");
        return Task.CompletedTask;
    }
}