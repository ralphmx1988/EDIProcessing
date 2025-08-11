using Microsoft.AspNetCore.Mvc;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;

namespace EDIProcessingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountTypesController : ControllerBase
{
    private readonly IAccountTypeRepository _accountTypeRepository;
    private readonly ILogger<AccountTypesController> _logger;

    public AccountTypesController(
        IAccountTypeRepository accountTypeRepository,
        ILogger<AccountTypesController> logger)
    {
        _accountTypeRepository = accountTypeRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get all account types
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountType>>> GetAccountTypes()
    {
        try
        {
            var accountTypes = await _accountTypeRepository.GetAllAsync();
            return Ok(accountTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account types");
            return StatusCode(500, "An error occurred while retrieving account types");
        }
    }

    /// <summary>
    /// Get all active account types
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<AccountType>>> GetActiveAccountTypes()
    {
        try
        {
            var accountTypes = await _accountTypeRepository.GetActiveAsync();
            return Ok(accountTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active account types");
            return StatusCode(500, "An error occurred while retrieving active account types");
        }
    }

    /// <summary>
    /// Get account type by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountType>> GetAccountType(int id)
    {
        try
        {
            var accountType = await _accountTypeRepository.GetByIdAsync(id);
            if (accountType == null)
            {
                return NotFound($"Account type with ID {id} not found");
            }

            return Ok(accountType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account type {AccountTypeId}", id);
            return StatusCode(500, "An error occurred while retrieving the account type");
        }
    }

    /// <summary>
    /// Get account type by code
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<AccountType>> GetAccountTypeByCode(string code)
    {
        try
        {
            var accountType = await _accountTypeRepository.GetByCodeAsync(code);
            if (accountType == null)
            {
                return NotFound($"Account type with code '{code}' not found");
            }

            return Ok(accountType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account type by code {Code}", code);
            return StatusCode(500, "An error occurred while retrieving the account type");
        }
    }

    /// <summary>
    /// Get account types with their account counts
    /// </summary>
    [HttpGet("with-counts")]
    public async Task<ActionResult<IEnumerable<object>>> GetAccountTypesWithCounts()
    {
        try
        {
            var result = await _accountTypeRepository.GetAccountTypesWithCountAsync();
            var response = result.Select(x => new
            {
                AccountType = x.AccountType,
                AccountCount = x.AccountCount
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account types with counts");
            return StatusCode(500, "An error occurred while retrieving account types with counts");
        }
    }

    /// <summary>
    /// Create a new account type
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AccountType>> CreateAccountType([FromBody] CreateAccountTypeRequest request)
    {
        try
        {
            // Check if code already exists
            if (await _accountTypeRepository.ExistsByCodeAsync(request.Code))
            {
                return BadRequest($"Account type with code '{request.Code}' already exists");
            }

            var accountType = new AccountType
            {
                Name = request.Name,
                Code = request.Code.ToUpper(),
                Description = request.Description,
                IsActive = request.IsActive ?? true
            };

            var createdAccountType = await _accountTypeRepository.AddAsync(accountType);

            return CreatedAtAction(
                nameof(GetAccountType),
                new { id = createdAccountType.Id },
                createdAccountType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account type");
            return StatusCode(500, "An error occurred while creating the account type");
        }
    }

    /// <summary>
    /// Update an existing account type
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAccountType(int id, [FromBody] UpdateAccountTypeRequest request)
    {
        try
        {
            var existingAccountType = await _accountTypeRepository.GetByIdAsync(id);
            if (existingAccountType == null)
            {
                return NotFound($"Account type with ID {id} not found");
            }

            // Check if code already exists (excluding current record)
            var existingByCode = await _accountTypeRepository.GetByCodeAsync(request.Code);
            if (existingByCode != null && existingByCode.Id != id)
            {
                return BadRequest($"Account type with code '{request.Code}' already exists");
            }

            existingAccountType.Name = request.Name;
            existingAccountType.Code = request.Code.ToUpper();
            existingAccountType.Description = request.Description;
            existingAccountType.IsActive = request.IsActive ?? existingAccountType.IsActive;

            await _accountTypeRepository.UpdateAsync(existingAccountType);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account type {AccountTypeId}", id);
            return StatusCode(500, "An error occurred while updating the account type");
        }
    }

    /// <summary>
    /// Delete an account type (only if no accounts are associated)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccountType(int id)
    {
        try
        {
            var deleted = await _accountTypeRepository.DeleteAsync(id);
            if (!deleted)
            {
                return BadRequest("Cannot delete account type. Either it doesn't exist or has associated accounts.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting account type {AccountTypeId}", id);
            return StatusCode(500, "An error occurred while deleting the account type");
        }
    }
}

/// <summary>
/// Request model for creating account types
/// </summary>
public class CreateAccountTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}

/// <summary>
/// Request model for updating account types
/// </summary>
public class UpdateAccountTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}
