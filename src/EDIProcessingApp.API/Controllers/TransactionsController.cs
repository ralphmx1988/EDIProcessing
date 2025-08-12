using Microsoft.AspNetCore.Mvc;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;

namespace EDIProcessingApp.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class TransactionsController(
    ITransactionRepository transactionRepository,
    IEdiProcessingService ediProcessingService,
    ILogger<TransactionsController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(
        [FromQuery] string? status = null,
        [FromQuery] Guid? accountId = null,
        [FromQuery] string? partnerId = null)
    {
        try
        {
            IEnumerable<Transaction> transactions;

            if (accountId.HasValue)
            {
                transactions = await transactionRepository.GetByAccountIdAsync(accountId.Value);
            }
            else if (!string.IsNullOrEmpty(partnerId))
            {
                transactions = await transactionRepository.GetByPartnerIdAsync(partnerId);
            }
            else if (!string.IsNullOrEmpty(status))
            {
                transactions = await transactionRepository.GetByStatusAsync(status);
            }
            else
            {
                transactions = await transactionRepository.GetAllAsync();
            }

            return Ok(transactions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Transaction>> GetTransaction(Guid id)
    {
        try
        {
            var transaction = await transactionRepository.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving transaction {TransactionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/acknowledge")]
    public async Task<ActionResult<AcknowledgeResponse>> SendAcknowledgment(Guid id)
    {
        try
        {
            var transaction = await transactionRepository.GetByIdAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            var success = await ediProcessingService.SendAcknowledgmentAsync(transaction);
            
            var response = new AcknowledgeResponse(
                transaction.Id,
                success,
                transaction.Status
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending acknowledgment for transaction {TransactionId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("by-partner/{partnerId}")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByPartner(string partnerId)
    {
        try
        {
            var transactions = await transactionRepository.GetByPartnerIdAsync(partnerId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving transactions for partner {PartnerId}", partnerId);
            return StatusCode(500, "Internal server error");
        }
    }
}

public record AcknowledgeResponse(Guid TransactionId, bool Success, string Status);
