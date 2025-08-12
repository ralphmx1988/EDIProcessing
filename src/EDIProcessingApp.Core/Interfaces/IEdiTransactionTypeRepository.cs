using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Core.Interfaces;

public interface IEdiTransactionTypeRepository : IRepository<EdiTransactionType>
{
    Task<EdiTransactionType?> GetByIdIntAsync(int id);
    Task<EdiTransactionType?> GetByX12CodeAsync(string x12Code);
    Task<EdiTransactionType?> GetByEdifactNameAsync(string edifactName);
    Task<IEnumerable<EdiTransactionType>> GetByDirectionAsync(string direction);
    Task<IEnumerable<EdiTransactionType>> GetActiveTransactionTypesAsync();
    Task<bool> IsX12CodeUniqueAsync(string x12Code, int? excludeId = null);
}
