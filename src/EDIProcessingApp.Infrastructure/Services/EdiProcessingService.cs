using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;
using EDIProcessingApp.Core.Helpers;

namespace EDIProcessingApp.Infrastructure.Services;

public class EdiProcessingService : IEdiProcessingService
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IEdiFileRepository _ediFileRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<EdiProcessingService> _logger;

    public EdiProcessingService(
        IFileStorageService fileStorageService,
        IEdiFileRepository ediFileRepository,
        ITransactionRepository transactionRepository,
        ILogger<EdiProcessingService> logger)
    {
        _fileStorageService = fileStorageService;
        _ediFileRepository = ediFileRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<EdiFile> ProcessFileAsync(Stream fileStream, string fileName, string source, Guid? accountId = null)
    {
        try
        {
            _logger.LogInformation("Processing EDI file: {FileName} from source: {Source}", fileName, source);

            // Calculate file hash
            var fileHash = await CalculateFileHashAsync(fileStream);
            var fileSizeBytes = fileStream.Length;

            // Upload file to storage
            var fileLocation = await _fileStorageService.UploadFileAsync(fileStream, fileName);

            // Determine file type and transaction type from filename or content
            var (fileType, transactionType) = DetermineFileTypeAndTransaction(fileName);

            // Create EDI file record
            var ediFile = new EdiFile
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                FileType = fileType,
                TransactionType = transactionType,
                Source = source,
                ReceivedAt = DateTime.UtcNow,
                Status = "Received", // Changed from "Pending" to "Received"
                FileLocation = fileLocation,
                FileSizeBytes = fileSizeBytes,
                FileHash = fileHash,
                AccountId = accountId
            };

            await _ediFileRepository.AddAsync(ediFile);
            await _ediFileRepository.SaveChangesAsync();

            // Automatically create a transaction for every received file
            var transaction = await CreateTransactionFromFileAsync(ediFile);

            _logger.LogInformation("EDI file {FileName} processed successfully with ID: {FileId} and Transaction ID: {TransactionId}", 
                fileName, ediFile.Id, transaction.Id);

            return ediFile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing EDI file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<bool> ValidateEdiFileAsync(EdiFile ediFile)
    {
        try
        {
            _logger.LogInformation("Validating EDI file: {FileId}", ediFile.Id);

            // Download file content for validation
            using var fileStream = await _fileStorageService.DownloadFileAsync(ediFile.FileLocation);
            using var reader = new StreamReader(fileStream);
            var content = await reader.ReadToEndAsync();

            // Basic validation rules
            var isValid = true;
            var errorMessages = new List<string>();

            // Check if file is not empty
            if (string.IsNullOrWhiteSpace(content))
            {
                isValid = false;
                errorMessages.Add("File is empty");
            }

            // Validate based on file type
            if (ediFile.FileType == "X12")
            {
                isValid = ValidateX12Format(content, errorMessages);
            }
            else if (ediFile.FileType == "EDIFACT")
            {
                isValid = ValidateEDIFACTFormat(content, errorMessages);
            }

            // Update file status
            ediFile.Status = isValid ? "Validated" : "Error";
            if (!isValid)
            {
                ediFile.ErrorMessage = string.Join("; ", errorMessages);
            }

            await _ediFileRepository.UpdateAsync(ediFile);
            await _ediFileRepository.SaveChangesAsync();

            _logger.LogInformation("EDI file {FileId} validation completed. Valid: {IsValid}", ediFile.Id, isValid);

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating EDI file: {FileId}", ediFile.Id);
            
            ediFile.Status = "Error";
            ediFile.ErrorMessage = $"Validation error: {ex.Message}";
            await _ediFileRepository.UpdateAsync(ediFile);
            await _ediFileRepository.SaveChangesAsync();
            
            return false;
        }
    }

    public async Task<Transaction> ParseTransactionAsync(EdiFile ediFile)
    {
        try
        {
            _logger.LogInformation("Parsing transaction from EDI file: {FileId}", ediFile.Id);

            // Get the existing transaction that was created when the file was received
            var existingTransactions = await _transactionRepository.GetByFileIdAsync(ediFile.Id);
            var transaction = existingTransactions?.FirstOrDefault();

            if (transaction == null)
            {
                // Fallback: create transaction if it doesn't exist (shouldn't happen with new flow)
                transaction = await CreateTransactionFromFileAsync(ediFile);
            }

            // Download file content
            using var fileStream = await _fileStorageService.DownloadFileAsync(ediFile.FileLocation);
            using var reader = new StreamReader(fileStream);
            var content = await reader.ReadToEndAsync();

            // Parse content based on file type
            var parsedData = ediFile.FileType switch
            {
                "X12" => ParseX12Content(content, ediFile.TransactionType),
                "EDIFACT" => ParseEDIFACTContent(content, ediFile.TransactionType),
                _ => new { Error = "Unsupported file type" }
            };

            // Update transaction with parsed data
            transaction.PartnerId = ExtractPartnerId(parsedData);
            transaction.Status = "Parsed";
            transaction.ProcessedAt = DateTime.UtcNow;
            transaction.JsonData = JsonSerializer.Serialize(parsedData);

            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            // Update file status
            ediFile.Status = "Processed";
            ediFile.ProcessedAt = DateTime.UtcNow;
            await _ediFileRepository.UpdateAsync(ediFile);
            await _ediFileRepository.SaveChangesAsync();

            _logger.LogInformation("Transaction updated successfully from EDI file: {FileId}, Transaction ID: {TransactionId}", 
                ediFile.Id, transaction.Id);

            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing transaction from EDI file: {FileId}", ediFile.Id);
            
            ediFile.Status = "Error";
            ediFile.ErrorMessage = $"Parsing error: {ex.Message}";
            await _ediFileRepository.UpdateAsync(ediFile);
            await _ediFileRepository.SaveChangesAsync();
            
            throw;
        }
    }

    public async Task<bool> SendAcknowledgmentAsync(Transaction transaction)
    {
        try
        {
            _logger.LogInformation("Sending acknowledgment for transaction: {TransactionId}", transaction.Id);

            // Generate acknowledgment based on transaction type
            var ackContent = GenerateAcknowledgment(transaction);

            // TODO: Send acknowledgment via appropriate channel (API, SFTP, etc.)
            // This is a placeholder implementation

            transaction.Status = "Acknowledged";
            await _transactionRepository.UpdateAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            _logger.LogInformation("Acknowledgment sent successfully for transaction: {TransactionId}", transaction.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending acknowledgment for transaction: {TransactionId}", transaction.Id);
            return false;
        }
    }

    private async Task<string> CalculateFileHashAsync(Stream stream)
    {
        using var sha256 = SHA256.Create();
        stream.Position = 0;
        var hashBytes = await sha256.ComputeHashAsync(stream);
        stream.Position = 0;
        return Convert.ToBase64String(hashBytes);
    }

    private async Task<Transaction> CreateTransactionFromFileAsync(EdiFile ediFile)
    {
        try
        {
            _logger.LogInformation("Creating transaction for EDI file: {FileId}", ediFile.Id);

            // Get transaction information from mapper
            var transactionInfo = EdiTransactionMapper.GetTransactionInfo(ediFile.TransactionType);

            // Create initial transaction record with basic information
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                FileId = ediFile.Id,
                TransactionType = ediFile.TransactionType,
                PartnerId = "AUTO_GENERATED", // Will be updated during parsing
                Status = "Received", // Initial status
                ProcessedAt = DateTime.UtcNow,
                JsonData = CreateInitialTransactionData(ediFile, transactionInfo),
                AccountId = ediFile.AccountId
            };

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();

            _logger.LogInformation("Transaction created successfully for file {FileId}: Transaction ID {TransactionId}", 
                ediFile.Id, transaction.Id);

            return transaction;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction for file: {FileId}", ediFile.Id);
            throw;
        }
    }

    private string CreateInitialTransactionData(EdiFile ediFile, dynamic? transactionInfo)
    {
        var initialData = new
        {
            FileId = ediFile.Id,
            FileName = ediFile.FileName,
            FileType = ediFile.FileType,
            TransactionType = ediFile.TransactionType,
            TransactionName = transactionInfo?.DocumentName ?? "Unknown",
            Description = transactionInfo?.Description ?? "Unknown transaction type",
            Source = ediFile.Source,
            ReceivedAt = ediFile.ReceivedAt,
            Status = "Received",
            ProcessingNotes = "Transaction automatically created upon file receipt",
            CreatedAt = DateTime.UtcNow
        };

        return JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true });
    }

    private (string fileType, string transactionType) DetermineFileTypeAndTransaction(string fileName)
    {
        var fileType = "X12"; // Default
        var transactionType = "850"; // Default (Purchase Order)

        // Determine file type
        if (fileName.Contains("EDIFACT", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("UNB", StringComparison.OrdinalIgnoreCase))
        {
            fileType = "EDIFACT";
        }

        // Extract transaction type from filename patterns
        var supportedX12Codes = EdiTransactionMapper.GetSupportedX12Codes();
        var supportedEdifactCodes = EdiTransactionMapper.GetSupportedEdifactCodes();

        // Check for X12 transaction codes
        foreach (var code in supportedX12Codes)
        {
            if (fileName.Contains(code, StringComparison.OrdinalIgnoreCase))
            {
                transactionType = code;
                fileType = "X12";
                break;
            }
        }

        // Check for EDIFACT message types
        foreach (var code in supportedEdifactCodes)
        {
            if (fileName.Contains(code, StringComparison.OrdinalIgnoreCase))
            {
                transactionType = code;
                fileType = "EDIFACT";
                break;
            }
        }

        return (fileType, transactionType);
    }

    private bool ValidateX12Format(string content, List<string> errorMessages)
    {
        // Basic X12 validation
        if (!content.StartsWith("ISA"))
        {
            errorMessages.Add("Invalid X12 format: Missing ISA header");
            return false;
        }

        if (!content.Contains("GS"))
        {
            errorMessages.Add("Invalid X12 format: Missing GS segment");
            return false;
        }

        if (!content.Contains("ST"))
        {
            errorMessages.Add("Invalid X12 format: Missing ST segment");
            return false;
        }

        // Validate transaction set code in ST segment
        var stIndex = content.IndexOf("ST*");
        if (stIndex >= 0)
        {
            var stSegment = content.Substring(stIndex, Math.Min(20, content.Length - stIndex));
            var parts = stSegment.Split('*');
            if (parts.Length >= 2)
            {
                var transactionCode = parts[1];
                if (!EdiTransactionMapper.IsValidTransactionType(transactionCode))
                {
                    errorMessages.Add($"Unsupported X12 transaction type: {transactionCode}");
                    return false;
                }
            }
        }

        return true;
    }

    private bool ValidateEDIFACTFormat(string content, List<string> errorMessages)
    {
        // Basic EDIFACT validation
        if (!content.StartsWith("UNA") && !content.StartsWith("UNB"))
        {
            errorMessages.Add("Invalid EDIFACT format: Missing UNA or UNB header");
            return false;
        }

        if (!content.Contains("UNH"))
        {
            errorMessages.Add("Invalid EDIFACT format: Missing UNH segment");
            return false;
        }

        // Validate message type in UNH segment
        var unhIndex = content.IndexOf("UNH+");
        if (unhIndex >= 0)
        {
            var unhSegment = content.Substring(unhIndex, Math.Min(50, content.Length - unhIndex));
            var messageTypeStart = unhSegment.IndexOf('+', 4);
            if (messageTypeStart > 0)
            {
                var messageTypePart = unhSegment.Substring(messageTypeStart + 1);
                var messageTypeEnd = messageTypePart.IndexOf('+');
                if (messageTypeEnd > 0)
                {
                    var messageType = messageTypePart.Substring(0, messageTypeEnd).Split(':')[0];
                    if (!EdiTransactionMapper.IsValidTransactionType(messageType))
                    {
                        errorMessages.Add($"Unsupported EDIFACT message type: {messageType}");
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private object ParseX12Content(string content, string transactionType)
    {
        // Enhanced X12 parsing with transaction type mapping
        var transactionInfo = EdiTransactionMapper.GetTransactionInfo(transactionType);
        
        return new
        {
            Format = "X12",
            TransactionType = transactionType,
            TransactionName = transactionInfo?.DocumentName ?? "Unknown",
            Description = transactionInfo?.Description ?? "Unknown transaction type",
            Content = content.Substring(0, Math.Min(1000, content.Length)), // First 1000 chars
            ParsedAt = DateTime.UtcNow,
            IsaSegment = ExtractSegment(content, "ISA"),
            GsSegment = ExtractSegment(content, "GS"),
            StSegment = ExtractSegment(content, "ST")
        };
    }

    private object ParseEDIFACTContent(string content, string transactionType)
    {
        // Enhanced EDIFACT parsing with message type mapping
        var transactionInfo = EdiTransactionMapper.GetTransactionInfo(transactionType);
        
        return new
        {
            Format = "EDIFACT",
            MessageType = transactionType,
            MessageName = transactionInfo?.DocumentName ?? "Unknown",
            Description = transactionInfo?.Description ?? "Unknown message type",
            Content = content.Substring(0, Math.Min(1000, content.Length)), // First 1000 chars
            ParsedAt = DateTime.UtcNow,
            UnbSegment = ExtractSegment(content, "UNB"),
            UnhSegment = ExtractSegment(content, "UNH")
        };
    }

    private string? ExtractSegment(string content, string segmentId)
    {
        var index = content.IndexOf(segmentId);
        if (index < 0) return null;

        var endIndex = content.IndexOf('~', index); // X12 segment terminator
        if (endIndex < 0)
        {
            endIndex = content.IndexOf('\'', index); // EDIFACT segment terminator
        }
        
        if (endIndex < 0) return null;

        return content.Substring(index, endIndex - index + 1);
    }

    private string ExtractPartnerId(object parsedData)
    {
        // Extract partner ID from parsed data
        // This is a simplified implementation
        return "PARTNER001";
    }

    private string GenerateAcknowledgment(Transaction transaction)
    {
        // Generate appropriate acknowledgment
        return $"ACK_{transaction.TransactionType}_{transaction.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}";
    }
}
