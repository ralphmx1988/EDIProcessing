using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDIProcessingApp.Core.Entities;

public class EdiTransactionType
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string X12Code { get; set; } = string.Empty; // 850, 810, 856, etc.
    
    [Required]
    [MaxLength(50)]
    public string DocumentName { get; set; } = string.Empty; // Purchase Order, Invoice, etc.
    
    [Required]
    [MaxLength(20)]
    public string EdifactName { get; set; } = string.Empty; // ORDERS, INVOIC, DESADV, etc.
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty; // Order request, Billing information, etc.
    
    [Required]
    [MaxLength(50)]
    public string Direction { get; set; } = "Both"; // Inbound, Outbound, Both
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<EdiFile> EdiFiles { get; set; } = new List<EdiFile>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
