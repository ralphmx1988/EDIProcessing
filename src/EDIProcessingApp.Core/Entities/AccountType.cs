using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EDIProcessingApp.Core.Entities;

/// <summary>
/// Represents different types of accounts in the EDI processing system
/// </summary>
public class AccountType
{
    /// <summary>
    /// Unique identifier for the account type
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the account type (e.g., Customer, Vendor, Partner)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the account type
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Code identifier for the account type (e.g., CUST, VEND, PART)
    /// </summary>
    [Required]
    [MaxLength(10)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this account type is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Date when the account type was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date when the account type was last updated
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Navigation property for accounts of this type
    /// </summary>
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
