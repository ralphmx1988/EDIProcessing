using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EDIProcessingApp.Core.Entities;
using EDIProcessingApp.Core.Interfaces;
using EDIProcessingApp.Infrastructure.Data;

namespace EDIProcessingApp.Infrastructure.Repositories;

public class EdiTransactionTypeRepository : Repository<EdiTransactionType>, IEdiTransactionTypeRepository
{
    public EdiTransactionTypeRepository(EDIProcessingDbContext context) : base(context)
    {
    }

    public async Task<EdiTransactionType?> GetByIdIntAsync(int id)
    {
        return await _context.EdiTransactionTypes.FindAsync(id);
    }

    public async Task<EdiTransactionType?> GetByX12CodeAsync(string x12Code)
    {
        return await _context.EdiTransactionTypes
            .FirstOrDefaultAsync(e => e.X12Code == x12Code && e.IsActive);
    }

    public async Task<EdiTransactionType?> GetByEdifactNameAsync(string edifactName)
    {
        return await _context.EdiTransactionTypes
            .FirstOrDefaultAsync(e => e.EdifactName == edifactName && e.IsActive);
    }

    public async Task<IEnumerable<EdiTransactionType>> GetByDirectionAsync(string direction)
    {
        return await _context.EdiTransactionTypes
            .Where(e => e.IsActive && (e.Direction == direction || e.Direction == "Both"))
            .OrderBy(e => e.DocumentName)
            .ToListAsync();
    }

    public async Task<IEnumerable<EdiTransactionType>> GetActiveTransactionTypesAsync()
    {
        return await _context.EdiTransactionTypes
            .Where(e => e.IsActive)
            .OrderBy(e => e.X12Code)
            .ToListAsync();
    }

    public async Task<bool> IsX12CodeUniqueAsync(string x12Code, int? excludeId = null)
    {
        var query = _context.EdiTransactionTypes
            .Where(e => e.X12Code == x12Code);

        if (excludeId.HasValue)
        {
            query = query.Where(e => e.Id != excludeId.Value);
        }

        return !await query.AnyAsync();
    }
}
