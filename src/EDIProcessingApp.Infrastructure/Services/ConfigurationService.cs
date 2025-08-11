using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;

namespace EDIProcessingApp.Infrastructure.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(
        IAccountRepository accountRepository,
        ILogger<ConfigurationService> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<string?> GetAccountConfigurationAsync(Guid accountId, string configurationKey)
    {
        try
        {
            var configuration = await _accountRepository.GetAccountConfigurationAsync(accountId, configurationKey);
            
            if (configuration?.IsEncrypted == true)
            {
                // TODO: Implement decryption logic
                return DecryptValue(configuration.ConfigurationValue);
            }

            return configuration?.ConfigurationValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting configuration {ConfigKey} for account {AccountId}", configurationKey, accountId);
            return null;
        }
    }

    public async Task SetAccountConfigurationAsync(Guid accountId, string configurationKey, string value, string configurationType = "General")
    {
        try
        {
            var existingConfig = await _accountRepository.GetAccountConfigurationAsync(accountId, configurationKey);
            
            if (existingConfig != null)
            {
                existingConfig.ConfigurationValue = ShouldEncrypt(configurationKey) ? EncryptValue(value) : value;
                existingConfig.IsEncrypted = ShouldEncrypt(configurationKey);
                existingConfig.UpdatedAt = DateTime.UtcNow;
                
                // Note: Update would be handled by the repository
            }
            else
            {
                var newConfig = new AccountConfiguration
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    ConfigurationKey = configurationKey,
                    ConfigurationValue = ShouldEncrypt(configurationKey) ? EncryptValue(value) : value,
                    ConfigurationType = configurationType,
                    IsEncrypted = ShouldEncrypt(configurationKey),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _accountRepository.AddAsync(newConfig.Account); // This would need to be properly handled
                // Note: This is simplified - in reality, you'd have a specific AccountConfiguration repository
            }

            await _accountRepository.SaveChangesAsync();
            
            _logger.LogInformation("Configuration {ConfigKey} set for account {AccountId}", configurationKey, accountId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting configuration {ConfigKey} for account {AccountId}", configurationKey, accountId);
            throw;
        }
    }

    public async Task<bool> ValidateAccountConfigurationAsync(Guid accountId)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return false;
            }

            var configurations = await _accountRepository.GetAccountConfigurationsAsync(accountId);
            
            // Check for required configurations
            var requiredConfigs = new[]
            {
                "EDI.FileFormat",
                "SFTP.HostName",
                "SFTP.UserName",
                "API.EndpointUrl"
            };

            foreach (var requiredConfig in requiredConfigs)
            {
                var config = await _accountRepository.GetAccountConfigurationAsync(accountId, requiredConfig);
                if (config == null || string.IsNullOrWhiteSpace(config.ConfigurationValue))
                {
                    _logger.LogWarning("Missing required configuration {ConfigKey} for account {AccountId}", requiredConfig, accountId);
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating configuration for account {AccountId}", accountId);
            return false;
        }
    }

    private bool ShouldEncrypt(string configurationKey)
    {
        // Determine which configuration values should be encrypted
        var sensitiveKeys = new[]
        {
            "SFTP.Password",
            "API.ApiKey",
            "API.SecretKey",
            "Database.ConnectionString",
            "Security.PrivateKey"
        };

        return Array.Exists(sensitiveKeys, key => 
            configurationKey.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    private string EncryptValue(string value)
    {
        // TODO: Implement proper encryption using Azure Key Vault or similar
        // This is a placeholder implementation
        var bytes = System.Text.Encoding.UTF8.GetBytes(value);
        return Convert.ToBase64String(bytes);
    }

    private string DecryptValue(string encryptedValue)
    {
        // TODO: Implement proper decryption using Azure Key Vault or similar
        // This is a placeholder implementation
        try
        {
            var bytes = Convert.FromBase64String(encryptedValue);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return encryptedValue; // Return as-is if decryption fails
        }
    }
}
