using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;
using EDIProcessingApp.Infrastructure.Data;

namespace EDIProcessingApp.Infrastructure.Repositories;

public class AccountTypeRepository : IAccountTypeRepository
{
    private readonly EDIProcessingDbContext _context;

    public AccountTypeRepository(EDIProcessingDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountType>> GetAllAsync()
    {
        return await _context.AccountTypes
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<AccountType>> GetActiveAsync()
    {
        return await _context.AccountTypes
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<AccountType?> GetByIdAsync(int id)
    {
        return await _context.AccountTypes
            .Include(x => x.Accounts)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AccountType?> GetByCodeAsync(string code)
    {
        return await _context.AccountTypes
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.AccountTypes
            .AnyAsync(x => x.Code == code);
    }

    public async Task<AccountType> AddAsync(AccountType accountType)
    {
        _context.AccountTypes.Add(accountType);
        await _context.SaveChangesAsync();
        return accountType;
    }

    public async Task UpdateAsync(AccountType accountType)
    {
        _context.AccountTypes.Update(accountType);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var accountType = await _context.AccountTypes
            .Include(x => x.Accounts)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (accountType == null)
            return false;

        // Check if there are any accounts using this type
        if (accountType.Accounts.Any())
            return false; // Cannot delete if accounts exist

        _context.AccountTypes.Remove(accountType);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<(AccountType AccountType, int AccountCount)>> GetAccountTypesWithCountAsync()
    {
        var result = await _context.AccountTypes
            .Select(at => new
            {
                AccountType = at,
                AccountCount = at.Accounts.Count()
            })
            .OrderBy(x => x.AccountType.Name)
            .ToListAsync();

        return result.Select(x => (x.AccountType, x.AccountCount));
    }
}
