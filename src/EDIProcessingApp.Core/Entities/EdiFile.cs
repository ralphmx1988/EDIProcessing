using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDIProcessingApp.Core.Entities;

public class EdiFile
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string FileType { get; set; } = string.Empty; // X12, EDIFACT
    
    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // 850, 810, etc.
    
    [Required]
    [MaxLength(50)]
    public string Source { get; set; } = string.Empty; // SFTP, API
    
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ProcessedAt { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Processed, Error
    
    public string? ErrorMessage { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string FileLocation { get; set; } = string.Empty;
    
    public long FileSizeBytes { get; set; }
    
    public string? FileHash { get; set; }
    
    public Guid? AccountId { get; set; }
    
    // Navigation properties
    public virtual Account? Account { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
