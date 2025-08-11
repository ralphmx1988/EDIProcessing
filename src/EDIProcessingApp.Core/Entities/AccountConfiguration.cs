using System;
using System.ComponentModel.DataAnnotations;

namespace EDIProcessingApp.Core.Entities;

public class AccountConfiguration
{
    public Guid Id { get; set; }
    
    public Guid AccountId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ConfigurationKey { get; set; } = string.Empty;
    
    [Required]
    public string ConfigurationValue { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Description { get; set; }
    
    [MaxLength(50)]
    public string ConfigurationType { get; set; } = "General"; // General, EDI, SFTP, Validation, etc.
    
    public bool IsEncrypted { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Account Account { get; set; } = null!;
}
