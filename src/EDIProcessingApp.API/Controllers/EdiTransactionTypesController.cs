using Microsoft.AspNetCore.Mvc;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;

namespace EDIProcessingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EdiTransactionTypesController : ControllerBase
{
    private readonly IEdiTransactionTypeRepository _repository;
    private readonly ILogger<EdiTransactionTypesController> _logger;

    public EdiTransactionTypesController(
        IEdiTransactionTypeRepository repository,
        ILogger<EdiTransactionTypesController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get all active EDI transaction types
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EdiTransactionTypeDto>>> GetEdiTransactionTypes()
    {
        try
        {
            var transactionTypes = await _repository.GetActiveTransactionTypesAsync();
            var result = transactionTypes.Select(MapToDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving EDI transaction types");
            return StatusCode(500, "An error occurred while retrieving EDI transaction types");
        }
    }

    /// <summary>
    /// Get EDI transaction type by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EdiTransactionTypeDto>> GetEdiTransactionType(int id)
    {
        try
        {
            var transactionType = await _repository.GetByIdIntAsync(id);
            if (transactionType == null)
            {
                return NotFound($"EDI transaction type with ID {id} not found");
            }

            return Ok(MapToDto(transactionType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving EDI transaction type {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the EDI transaction type");
        }
    }

    /// <summary>
    /// Get EDI transaction type by X12 code
    /// </summary>
    [HttpGet("x12/{x12Code}")]
    public async Task<ActionResult<EdiTransactionTypeDto>> GetByX12Code(string x12Code)
    {
        try
        {
            var transactionType = await _repository.GetByX12CodeAsync(x12Code);
            if (transactionType == null)
            {
                return NotFound($"EDI transaction type with X12 code {x12Code} not found");
            }

            return Ok(MapToDto(transactionType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving EDI transaction type by X12 code {X12Code}", x12Code);
            return StatusCode(500, "An error occurred while retrieving the EDI transaction type");
        }
    }

    /// <summary>
    /// Get EDI transaction type by EDIFACT name
    /// </summary>
    [HttpGet("edifact/{edifactName}")]
    public async Task<ActionResult<EdiTransactionTypeDto>> GetByEdifactName(string edifactName)
    {
        try
        {
            var transactionType = await _repository.GetByEdifactNameAsync(edifactName);
            if (transactionType == null)
            {
                return NotFound($"EDI transaction type with EDIFACT name {edifactName} not found");
            }

            return Ok(MapToDto(transactionType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving EDI transaction type by EDIFACT name {EdifactName}", edifactName);
            return StatusCode(500, "An error occurred while retrieving the EDI transaction type");
        }
    }

    /// <summary>
    /// Get EDI transaction types by direction (Inbound, Outbound, Both)
    /// </summary>
    [HttpGet("direction/{direction}")]
    public async Task<ActionResult<IEnumerable<EdiTransactionTypeDto>>> GetByDirection(string direction)
    {
        try
        {
            var validDirections = new[] { "Inbound", "Outbound", "Both" };
            if (!validDirections.Contains(direction, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest($"Invalid direction. Valid values are: {string.Join(", ", validDirections)}");
            }

            var transactionTypes = await _repository.GetByDirectionAsync(direction);
            var result = transactionTypes.Select(MapToDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving EDI transaction types by direction {Direction}", direction);
            return StatusCode(500, "An error occurred while retrieving EDI transaction types");
        }
    }

    /// <summary>
    /// Create a new EDI transaction type
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EdiTransactionTypeDto>> CreateEdiTransactionType(CreateEdiTransactionTypeDto createDto)
    {
        try
        {
            // Validate X12 code uniqueness
            var isUnique = await _repository.IsX12CodeUniqueAsync(createDto.X12Code);
            if (!isUnique)
            {
                return BadRequest($"X12 code '{createDto.X12Code}' already exists");
            }

            var transactionType = new EdiTransactionType
            {
                X12Code = createDto.X12Code,
                DocumentName = createDto.DocumentName,
                EdifactName = createDto.EdifactName,
                Description = createDto.Description,
                Direction = createDto.Direction ?? "Both",
                IsActive = createDto.IsActive ?? true,
                CreatedDate = DateTime.UtcNow
            };

            await _repository.AddAsync(transactionType);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Created new EDI transaction type: {X12Code} - {DocumentName}", 
                transactionType.X12Code, transactionType.DocumentName);

            return CreatedAtAction(nameof(GetEdiTransactionType), 
                new { id = transactionType.Id }, 
                MapToDto(transactionType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating EDI transaction type");
            return StatusCode(500, "An error occurred while creating the EDI transaction type");
        }
    }

    /// <summary>
    /// Update an existing EDI transaction type
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EdiTransactionTypeDto>> UpdateEdiTransactionType(int id, UpdateEdiTransactionTypeDto updateDto)
    {
        try
        {
            var transactionType = await _repository.GetByIdIntAsync(id);
            if (transactionType == null)
            {
                return NotFound($"EDI transaction type with ID {id} not found");
            }

            // Validate X12 code uniqueness if changed
            if (updateDto.X12Code != transactionType.X12Code)
            {
                var isUnique = await _repository.IsX12CodeUniqueAsync(updateDto.X12Code, id);
                if (!isUnique)
                {
                    return BadRequest($"X12 code '{updateDto.X12Code}' already exists");
                }
            }

            // Update properties
            transactionType.X12Code = updateDto.X12Code;
            transactionType.DocumentName = updateDto.DocumentName;
            transactionType.EdifactName = updateDto.EdifactName;
            transactionType.Description = updateDto.Description;
            transactionType.Direction = updateDto.Direction ?? transactionType.Direction;
            transactionType.IsActive = updateDto.IsActive ?? transactionType.IsActive;
            transactionType.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(transactionType);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Updated EDI transaction type: {Id} - {X12Code} - {DocumentName}", 
                transactionType.Id, transactionType.X12Code, transactionType.DocumentName);

            return Ok(MapToDto(transactionType));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating EDI transaction type {Id}", id);
            return StatusCode(500, "An error occurred while updating the EDI transaction type");
        }
    }

    /// <summary>
    /// Delete an EDI transaction type (soft delete by setting IsActive = false)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEdiTransactionType(int id)
    {
        try
        {
            var transactionType = await _repository.GetByIdIntAsync(id);
            if (transactionType == null)
            {
                return NotFound($"EDI transaction type with ID {id} not found");
            }

            // Soft delete - set IsActive to false
            transactionType.IsActive = false;
            transactionType.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(transactionType);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Soft deleted EDI transaction type: {Id} - {X12Code} - {DocumentName}", 
                transactionType.Id, transactionType.X12Code, transactionType.DocumentName);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting EDI transaction type {Id}", id);
            return StatusCode(500, "An error occurred while deleting the EDI transaction type");
        }
    }

    private static EdiTransactionTypeDto MapToDto(EdiTransactionType entity)
    {
        return new EdiTransactionTypeDto
        {
            Id = entity.Id,
            X12Code = entity.X12Code,
            DocumentName = entity.DocumentName,
            EdifactName = entity.EdifactName,
            Description = entity.Description,
            Direction = entity.Direction,
            IsActive = entity.IsActive,
            CreatedDate = entity.CreatedDate,
            UpdatedDate = entity.UpdatedDate
        };
    }
}

// DTOs
public class EdiTransactionTypeDto
{
    public int Id { get; set; }
    public string X12Code { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public string EdifactName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}

public class CreateEdiTransactionTypeDto
{
    public string X12Code { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public string EdifactName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Direction { get; set; } = "Both";
    public bool? IsActive { get; set; } = true;
}

public class UpdateEdiTransactionTypeDto
{
    public string X12Code { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public string EdifactName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Direction { get; set; }
    public bool? IsActive { get; set; }
}
