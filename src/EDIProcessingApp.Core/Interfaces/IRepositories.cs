using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Core.Interfaces;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByCodeAsync(string code);
    Task<IEnumerable<Account>> GetActiveAccountsAsync();
    Task<IEnumerable<AccountConfiguration>> GetAccountConfigurationsAsync(Guid accountId);
    Task<AccountConfiguration?> GetAccountConfigurationAsync(Guid accountId, string configurationKey);
}

public interface IEdiFileRepository : IRepository<EdiFile>
{
    Task<IEnumerable<EdiFile>> GetByStatusAsync(string status);
    Task<IEnumerable<EdiFile>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<EdiFile>> GetPendingFilesAsync();
    Task<EdiFile?> GetByFileNameAsync(string fileName);
}

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByFileIdAsync(Guid fileId);
    Task<IEnumerable<Transaction>> GetByStatusAsync(string status);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);
    Task<IEnumerable<Transaction>> GetByPartnerIdAsync(string partnerId);
}
