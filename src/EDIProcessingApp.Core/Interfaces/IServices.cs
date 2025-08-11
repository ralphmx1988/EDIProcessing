using System;
using System.IO;
using System.Threading.Tasks;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Core.Interfaces;

public interface IEdiProcessingService
{
    Task<EdiFile> ProcessFileAsync(Stream fileStream, string fileName, string source, Guid? accountId = null);
    Task<bool> ValidateEdiFileAsync(EdiFile ediFile);
    Task<Transaction> ParseTransactionAsync(EdiFile ediFile);
    Task<bool> SendAcknowledgmentAsync(Transaction transaction);
}

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName = "edi-files");
    Task<Stream> DownloadFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    string GetFileUrl(string filePath, TimeSpan? expiry = null);
    Task<string> GetFileUrlAsync(string filePath, TimeSpan? expiry = null);
}

public interface INotificationService
{
    Task SendProcessingNotificationAsync(EdiFile ediFile);
    Task SendErrorNotificationAsync(EdiFile ediFile, string error);
    Task SendTransactionNotificationAsync(Transaction transaction);
}

public interface IConfigurationService
{
    Task<string?> GetAccountConfigurationAsync(Guid accountId, string configurationKey);
    Task SetAccountConfigurationAsync(Guid accountId, string configurationKey, string value, string configurationType = "General");
    Task<bool> ValidateAccountConfigurationAsync(Guid accountId);
}
