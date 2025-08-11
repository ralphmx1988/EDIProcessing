using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;

namespace EDIProcessingApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EdiFilesController : ControllerBase
{
    private readonly IEdiProcessingService _ediProcessingService;
    private readonly IEdiFileRepository _ediFileRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<EdiFilesController> _logger;

    public EdiFilesController(
        IEdiProcessingService ediProcessingService,
        IEdiFileRepository ediFileRepository,
        ITransactionRepository transactionRepository,
        ILogger<EdiFilesController> logger)
    {
        _ediProcessingService = ediProcessingService;
        _ediFileRepository = ediFileRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<EdiFileUploadResponse>> UploadFile(IFormFile file, [FromQuery] Guid? accountId = null)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided");
            }

            _logger.LogInformation("Uploading EDI file: {FileName}", file.FileName);

            using var stream = file.OpenReadStream();
            var ediFile = await _ediProcessingService.ProcessFileAsync(stream, file.FileName, "API", accountId);

            // Get the automatically created transaction
            var transactions = await _transactionRepository.GetByFileIdAsync(ediFile.Id);
            var transaction = transactions?.FirstOrDefault();

            var response = new EdiFileUploadResponse(
                ediFile.Id,
                ediFile.FileName,
                ediFile.Status,
                ediFile.ReceivedAt,
                transaction?.Id,
                transaction?.Status
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file?.FileName);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EdiFile>>> GetFiles([FromQuery] string? status = null, [FromQuery] Guid? accountId = null)
    {
        try
        {
            IEnumerable<EdiFile> files;

            if (accountId.HasValue)
            {
                files = await _ediFileRepository.GetByAccountIdAsync(accountId.Value);
            }
            else if (!string.IsNullOrEmpty(status))
            {
                files = await _ediFileRepository.GetByStatusAsync(status);
            }
            else
            {
                files = await _ediFileRepository.GetAllAsync();
            }

            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving EDI files");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EdiFileDetailResponse>> GetFile(Guid id)
    {
        try
        {
            var file = await _ediFileRepository.GetByIdAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            // Get associated transactions
            var transactions = await _transactionRepository.GetByFileIdAsync(id);

            var response = new EdiFileDetailResponse(
                file,
                transactions?.ToList() ?? new List<Transaction>()
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving EDI file {FileId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/validate")]
    public async Task<ActionResult<ValidateFileResponse>> ValidateFile(Guid id)
    {
        try
        {
            var ediFile = await _ediFileRepository.GetByIdAsync(id);
            if (ediFile == null)
            {
                return NotFound();
            }

            var isValid = await _ediProcessingService.ValidateEdiFileAsync(ediFile);
            
            var response = new ValidateFileResponse(
                ediFile.Id,
                isValid,
                ediFile.Status,
                ediFile.ErrorMessage
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating EDI file {FileId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/process")]
    public async Task<ActionResult<ProcessFileResponse>> ProcessFile(Guid id)
    {
        try
        {
            var ediFile = await _ediFileRepository.GetByIdAsync(id);
            if (ediFile == null)
            {
                return NotFound();
            }

            // First validate
            var isValid = await _ediProcessingService.ValidateEdiFileAsync(ediFile);
            if (!isValid)
            {
                return BadRequest("File validation failed");
            }

            // Then parse transaction
            var transaction = await _ediProcessingService.ParseTransactionAsync(ediFile);

            var response = new ProcessFileResponse(
                ediFile.Id,
                transaction.Id,
                ediFile.Status,
                transaction.Status
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing EDI file {FileId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}/transactions")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetFileTransactions(Guid id)
    {
        try
        {
            var transactions = await _transactionRepository.GetByFileIdAsync(id);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions for file {FileId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<EdiFile>>> GetPendingFiles()
    {
        try
        {
            var files = await _ediFileRepository.GetPendingFilesAsync();
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending files");
            return StatusCode(500, "Internal server error");
        }
    }
}

public record EdiFileUploadResponse(
    Guid FileId, 
    string FileName, 
    string FileStatus, 
    DateTime ReceivedAt, 
    Guid? TransactionId = null, 
    string? TransactionStatus = null);
public record EdiFileDetailResponse(EdiFile File, List<Transaction> Transactions);
public record ValidateFileResponse(Guid FileId, bool IsValid, string Status, string? ErrorMessage);
public record ProcessFileResponse(Guid FileId, Guid TransactionId, string FileStatus, string TransactionStatus);
