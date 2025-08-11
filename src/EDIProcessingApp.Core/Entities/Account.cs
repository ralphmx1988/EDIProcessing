using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIProcessingApp.Core.Entities;

public class Account
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Foreign key to AccountType
    /// </summary>
    public int AccountTypeId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    
    /// <summary>
    /// The type of this account (Customer, Vendor, Partner, etc.)
    /// </summary>
    [ForeignKey("AccountTypeId")]
    public virtual AccountType AccountType { get; set; } = null!;
    
    public virtual ICollection<AccountConfiguration> Configurations { get; set; } = new List<AccountConfiguration>();
    public virtual ICollection<EdiFile> EdiFiles { get; set; } = new List<EdiFile>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
