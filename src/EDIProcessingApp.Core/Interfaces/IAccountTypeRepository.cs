using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Core.Interfaces;

/// <summary>
/// Repository interface for AccountType entity operations
/// </summary>
public interface IAccountTypeRepository
{
    /// <summary>
    /// Get all account types
    /// </summary>
    Task<IEnumerable<AccountType>> GetAllAsync();

    /// <summary>
    /// Get all active account types
    /// </summary>
    Task<IEnumerable<AccountType>> GetActiveAsync();

    /// <summary>
    /// Get account type by ID
    /// </summary>
    Task<AccountType?> GetByIdAsync(int id);

    /// <summary>
    /// Get account type by code
    /// </summary>
    Task<AccountType?> GetByCodeAsync(string code);

    /// <summary>
    /// Check if account type exists by code
    /// </summary>
    Task<bool> ExistsByCodeAsync(string code);

    /// <summary>
    /// Add a new account type
    /// </summary>
    Task<AccountType> AddAsync(AccountType accountType);

    /// <summary>
    /// Update an existing account type
    /// </summary>
    Task UpdateAsync(AccountType accountType);

    /// <summary>
    /// Delete an account type (if no accounts are associated)
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Get account types with their associated accounts count
    /// </summary>
    Task<IEnumerable<(AccountType AccountType, int AccountCount)>> GetAccountTypesWithCountAsync();
}
