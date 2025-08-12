using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDIProcessingApp.Core.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    
    public Guid FileId { get; set; }
    
    public int? EdiTransactionTypeId { get; set; } // Foreign key to EdiTransactionType
    
    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // Purchase Order, Invoice, etc. (kept for backwards compatibility)
    
    [Required]
    [MaxLength(100)]
    public string PartnerId { get; set; } = string.Empty; // Sender/receiver identifier
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Validated, Error, Sent
    
    public string? ErrorMessage { get; set; }
    
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    public string JsonData { get; set; } = string.Empty; // Parsed transaction data in JSON format
    
    public Guid? AccountId { get; set; }
    
    // Navigation properties
    public virtual EdiFile File { get; set; } = null!;
    public virtual EdiTransactionType? EdiTransactionType { get; set; }
    public virtual Account? Account { get; set; }
}
