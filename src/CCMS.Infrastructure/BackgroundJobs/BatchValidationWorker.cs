using System;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CCMS.Infrastructure.BackgroundJobs
{
    public class BatchValidationWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BatchValidationWorker> _logger;

        public BatchValidationWorker(IServiceProvider serviceProvider, ILogger<BatchValidationWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BatchValidationWorker started at: {time}", DateTimeOffset.Now);

            // Run every 15 minutes
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("BatchValidationWorker running at: {time}", DateTimeOffset.Now);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var batchValidationService = scope.ServiceProvider.GetRequiredService<IBatchValidationService>();
                    
                    int count = await batchValidationService.ProcessPendingCasesAsync(stoppingToken);
                    
                    _logger.LogInformation("BatchValidationWorker processed {count} pending cases.", count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while running the BatchValidationWorker.");
                }
            }
        }
    }
}
