using Microsoft.AspNetCore.Mvc;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;

namespace EDIProcessingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(
        IAccountRepository accountRepository,
        IConfigurationService configurationService,
        ILogger<AccountsController> logger)
    {
        _accountRepository = accountRepository;
        _configurationService = configurationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
    {
        try
        {
            var accounts = await _accountRepository.GetActiveAccountsAsync();
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving accounts");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> GetAccount(Guid id)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account {AccountId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<Account>> GetAccountByCode(string code)
    {
        try
        {
            var account = await _accountRepository.GetByCodeAsync(code);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account with code {Code}", code);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Account>> CreateAccount(CreateAccountRequest request)
    {
        try
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _accountRepository.AddAsync(account);
            await _accountRepository.SaveChangesAsync();

            _logger.LogInformation("Account created: {AccountId}", account.Id);
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAccount(Guid id, UpdateAccountRequest request)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            account.Name = request.Name;
            account.Description = request.Description;
            account.IsActive = request.IsActive;
            account.UpdatedAt = DateTime.UtcNow;

            await _accountRepository.UpdateAsync(account);
            await _accountRepository.SaveChangesAsync();

            _logger.LogInformation("Account updated: {AccountId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account {AccountId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}/configurations")]
    public async Task<ActionResult<IEnumerable<AccountConfiguration>>> GetAccountConfigurations(Guid id)
    {
        try
        {
            var configurations = await _accountRepository.GetAccountConfigurationsAsync(id);
            return Ok(configurations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving configurations for account {AccountId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/configurations")]
    public async Task<IActionResult> SetAccountConfiguration(Guid id, SetConfigurationRequest request)
    {
        try
        {
            await _configurationService.SetAccountConfigurationAsync(
                id, 
                request.ConfigurationKey, 
                request.ConfigurationValue, 
                request.ConfigurationType ?? "General");

            _logger.LogInformation("Configuration set for account {AccountId}: {ConfigKey}", id, request.ConfigurationKey);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting configuration for account {AccountId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

public record CreateAccountRequest(string Name, string Code, string? Description);
public record UpdateAccountRequest(string Name, string? Description, bool IsActive);
public record SetConfigurationRequest(string ConfigurationKey, string ConfigurationValue, string? ConfigurationType);
