using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;
using EDIProcessingApp.Infrastructure.Data;

namespace EDIProcessingApp.Infrastructure.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(EDIProcessingDbContext context) : base(context)
    {
    }

    public async Task<Account?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(x => x.Configurations)
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<IEnumerable<Account>> GetActiveAccountsAsync()
    {
        return await _dbSet
            .Where(x => x.IsActive)
            .Include(x => x.Configurations)
            .ToListAsync();
    }

    public async Task<IEnumerable<AccountConfiguration>> GetAccountConfigurationsAsync(Guid accountId)
    {
        return await _context.AccountConfigurations
            .Where(x => x.AccountId == accountId)
            .ToListAsync();
    }

    public async Task<AccountConfiguration?> GetAccountConfigurationAsync(Guid accountId, string configurationKey)
    {
        return await _context.AccountConfigurations
            .FirstOrDefaultAsync(x => x.AccountId == accountId && x.ConfigurationKey == configurationKey);
    }
}

public class EdiFileRepository : Repository<EdiFile>, IEdiFileRepository
{
    public EdiFileRepository(EDIProcessingDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EdiFile>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Where(x => x.Status == status)
            .Include(x => x.Account)
            .ToListAsync();
    }

    public async Task<IEnumerable<EdiFile>> GetByAccountIdAsync(Guid accountId)
    {
        return await _dbSet
            .Where(x => x.AccountId == accountId)
            .Include(x => x.Transactions)
            .OrderByDescending(x => x.ReceivedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<EdiFile>> GetPendingFilesAsync()
    {
        return await _dbSet
            .Where(x => x.Status == "Pending")
            .Include(x => x.Account)
            .OrderBy(x => x.ReceivedAt)
            .ToListAsync();
    }

    public async Task<EdiFile?> GetByFileNameAsync(string fileName)
    {
        return await _dbSet
            .FirstOrDefaultAsync(x => x.FileName == fileName);
    }
}

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(EDIProcessingDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Transaction>> GetByFileIdAsync(Guid fileId)
    {
        return await _dbSet
            .Where(x => x.FileId == fileId)
            .Include(x => x.File)
            .Include(x => x.Account)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Where(x => x.Status == status)
            .Include(x => x.File)
            .Include(x => x.Account)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId)
    {
        return await _dbSet
            .Where(x => x.AccountId == accountId)
            .Include(x => x.File)
            .OrderByDescending(x => x.ProcessedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByPartnerIdAsync(string partnerId)
    {
        return await _dbSet
            .Where(x => x.PartnerId == partnerId)
            .Include(x => x.File)
            .Include(x => x.Account)
            .OrderByDescending(x => x.ProcessedAt)
            .ToListAsync();
    }
}
