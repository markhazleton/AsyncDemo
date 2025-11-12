using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AsyncDemo.Web.Services;

/// <summary>
/// Health check for configuration validation
/// </summary>
public class ConfigurationHealthCheck : IHealthCheck
{
    private readonly IConfigurationValidationService _configValidationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationHealthCheck"/> class
    /// </summary>
    /// <param name="configValidationService">The configuration validation service</param>
    public ConfigurationHealthCheck(IConfigurationValidationService configValidationService)
    {
        _configValidationService = configValidationService ?? throw new ArgumentNullException(nameof(configValidationService));
    }

    /// <summary>
    /// Checks the health of the application configuration
    /// </summary>
    /// <param name="context">The health check context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the health check result</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _configValidationService.ValidateConfiguration();

            if (result.IsValid)
            {
                return Task.FromResult(HealthCheckResult.Healthy("Configuration is valid"));
            }

            var errorMessage = $"Configuration errors: {string.Join(", ", result.Errors)}";
            if (result.Warnings.Any())
            {
                errorMessage += $"; Warnings: {string.Join(", ", result.Warnings)}";
            }

            return Task.FromResult(HealthCheckResult.Unhealthy(errorMessage));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Configuration check failed: {ex.Message}"));
        }
    }
}