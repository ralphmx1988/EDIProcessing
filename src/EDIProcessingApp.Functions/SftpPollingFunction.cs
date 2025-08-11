using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EDIProcessingApp.Functions;

public class SftpPollingFunction
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SftpPollingFunction> _logger;

    public SftpPollingFunction(
        IConfiguration configuration,
        ILogger<SftpPollingFunction> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [Function("PollSftpServers")]
    public async Task PollSftpServers([TimerTrigger("0 */10 * * * *")] TimerInfo timer)
    {
        _logger.LogInformation("SFTP polling function executed at: {time}", DateTime.Now);

        try
        {
            // TODO: Implement SFTP polling logic
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SFTP polling function");
        }
    }

    private bool IsEdiFile(string fileName)
    {
        var ediExtensions = new[] { ".edi", ".x12", ".txt", ".dat", ".xml" };
        var extension = Path.GetExtension(fileName).ToLower();
        
        // Check by file extension
        if (Array.Exists(ediExtensions, ext => ext == extension))
        {
            return true;
        }

        // Check for X12 transaction codes in filename
        var x12Codes = new[] { "850", "810", "856", "855", "820", "862", "997" };
        foreach (var code in x12Codes)
        {
            if (fileName.Contains(code, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        // Check for EDIFACT message types in filename
        var edifactTypes = new[] { "ORDERS", "INVOIC", "DESADV", "ORDRSP", "PAYMUL", "REMADV", "DELFOR", "CONTRL" };
        foreach (var type in edifactTypes)
        {
            if (fileName.Contains(type, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
