using Microsoft.AspNetCore.Mvc;
using EDIProcessingApp.Core.Helpers;

namespace EDIProcessingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionTypesController : ControllerBase
{
    private readonly ILogger<TransactionTypesController> _logger;

    public TransactionTypesController(ILogger<TransactionTypesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<EdiTransactionInfo>> GetAllTransactionTypes()
    {
        try
        {
            var transactionTypes = EdiTransactionMapper.GetAllTransactionTypes();
            return Ok(transactionTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction types");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("x12")]
    public ActionResult<IEnumerable<string>> GetSupportedX12Codes()
    {
        try
        {
            var codes = EdiTransactionMapper.GetSupportedX12Codes();
            return Ok(codes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving X12 codes");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("edifact")]
    public ActionResult<IEnumerable<string>> GetSupportedEdifactCodes()
    {
        try
        {
            var codes = EdiTransactionMapper.GetSupportedEdifactCodes();
            return Ok(codes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving EDIFACT codes");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("lookup/{code}")]
    public ActionResult<EdiTransactionInfo> GetTransactionInfo(string code)
    {
        try
        {
            var info = EdiTransactionMapper.GetTransactionInfo(code);
            if (info == null)
            {
                return NotFound($"Transaction type '{code}' not found");
            }
            return Ok(info);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up transaction type {Code}", code);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("validate/{code}")]
    public ActionResult<ValidationResponse> ValidateTransactionType(string code)
    {
        try
        {
            var isValid = EdiTransactionMapper.IsValidTransactionType(code);
            var info = EdiTransactionMapper.GetTransactionInfo(code);
            
            var response = new ValidationResponse(
                code,
                isValid,
                info?.DocumentName,
                info?.Description
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating transaction type {Code}", code);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("map/x12-to-edifact/{x12Code}")]
    public ActionResult<string> MapX12ToEdifact(string x12Code)
    {
        try
        {
            var edifactName = EdiTransactionMapper.GetEdifactName(x12Code);
            return Ok(new { X12Code = x12Code, EdifactName = edifactName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping X12 code {Code} to EDIFACT", x12Code);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("map/edifact-to-x12/{edifactName}")]
    public ActionResult<string> MapEdifactToX12(string edifactName)
    {
        try
        {
            var x12Code = EdiTransactionMapper.GetX12Code(edifactName);
            return Ok(new { EdifactName = edifactName, X12Code = x12Code });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping EDIFACT name {Name} to X12", edifactName);
            return StatusCode(500, "Internal server error");
        }
    }
}

public record ValidationResponse(string Code, bool IsValid, string? DocumentName, string? Description);
