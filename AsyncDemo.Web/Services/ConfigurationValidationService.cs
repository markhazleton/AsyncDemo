using Microsoft.Extensions.Options;

namespace AsyncDemo.Web.Services;

/// <summary>
/// Service for validating application configuration
/// </summary>
public interface IConfigurationValidationService
{
    /// <summary>
    /// Validates the application configuration
    /// </summary>
    /// <returns>Validation result</returns>
    ConfigurationValidationResult ValidateConfiguration();
}

/// <summary>
/// Configuration validation result
/// </summary>
public class ConfigurationValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Implementation of configuration validation service
/// </summary>
public class ConfigurationValidationService : IConfigurationValidationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationValidationService> _logger;

    public ConfigurationValidationService(IConfiguration configuration, ILogger<ConfigurationValidationService> logger)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ConfigurationValidationResult ValidateConfiguration()
    {
     var result = new ConfigurationValidationResult { IsValid = true };

        try
        {
    // Validate OpenWeatherMap API Key
  var weatherApiKey = _configuration["OpenWeatherMapApiKey"];
    if (string.IsNullOrEmpty(weatherApiKey) || weatherApiKey == "KEYMISSING")
            {
   result.Warnings.Add("OpenWeatherMapApiKey is missing or set to default value. Weather functionality may not work properly.");
            }

      // Validate connection strings if any
  var connectionString = _configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(connectionString))
     {
              // Add connection string validation logic here
    _logger.LogInformation("Connection string found and will be validated");
       }

            // Validate required sections
    var asyncSection = _configuration.GetSection("Async");
          if (!asyncSection.Exists())
            {
   result.Warnings.Add("Async configuration section is missing. Default values will be used.");
      }

      // Log configuration validation results
        if (result.Warnings.Any())
            {
   _logger.LogWarning("Configuration validation completed with {WarningCount} warnings", result.Warnings.Count);
         foreach (var warning in result.Warnings)
          {
    _logger.LogWarning("Configuration warning: {Warning}", warning);
         }
            }

            if (result.Errors.Any())
       {
       result.IsValid = false;
             _logger.LogError("Configuration validation failed with {ErrorCount} errors", result.Errors.Count);
 foreach (var error in result.Errors)
           {
            _logger.LogError("Configuration error: {Error}", error);
    }
      }
            else
            {
    _logger.LogInformation("Configuration validation completed successfully");
            }
        }
        catch (Exception ex)
   {
      result.IsValid = false;
       result.Errors.Add($"Configuration validation failed: {ex.Message}");
      _logger.LogError(ex, "Error during configuration validation");
    }

        return result;
    }
}