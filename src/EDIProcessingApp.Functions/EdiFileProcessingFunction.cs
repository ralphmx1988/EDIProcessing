using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EDIProcessingApp.Functions;

public class EdiFileProcessingFunction
{
    private readonly ILogger<EdiFileProcessingFunction> _logger;

    public EdiFileProcessingFunction(ILogger<EdiFileProcessingFunction> logger)
    {
        _logger = logger;
    }

    [Function("ProcessPendingFiles")]
    public async Task ProcessPendingFiles([TimerTrigger("0 */5 * * * *")] TimerInfo timer)
    {
        _logger.LogInformation("Processing pending EDI files at: {time}", DateTime.Now);

        try
        {
            // TODO: Implement file processing logic
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessPendingFiles function");
        }
    }
}
