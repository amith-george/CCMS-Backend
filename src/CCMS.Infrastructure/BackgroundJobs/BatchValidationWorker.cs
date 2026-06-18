using System;
using System.Diagnostics;
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
 
            // Run every 15 minutes (The Scheduled Trigger)
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));
 
            do
            {
                _logger.LogInformation("BatchValidationWorker running at: {time}", DateTimeOffset.Now);
 
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var batchValidationService = scope.ServiceProvider.GetRequiredService<IBatchValidationService>();

                    // 2. START THE CLOCK
                    var stopwatch = Stopwatch.StartNew();
 
                    // 3. CAPTURE THE STATISTICS (count)
                    int count = await batchValidationService.ProcessPendingCasesAsync(stoppingToken);

                    // 4. STOP THE CLOCK
                    stopwatch.Stop();
 
                    // 5. LOG BOTH THE STATISTICS AND DURATION
                    _logger.LogInformation(
                        "BatchValidationWorker completed in {Duration}ms. Total cases processed: {Count}.", 
                        stopwatch.ElapsedMilliseconds, 
                        count
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while running the BatchValidationWorker.");
                }
            }
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
        }
    }
}
