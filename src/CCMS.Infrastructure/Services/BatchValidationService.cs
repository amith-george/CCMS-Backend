using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using CCMS.Application.Interfaces;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CCMS.Infrastructure.Services
{
    public class BatchValidationService : IBatchValidationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<BatchValidationService> _logger;

        public BatchValidationService(ApplicationDbContext context, IAuditLogService auditLogService, ILogger<BatchValidationService> logger)
        {
            _context = context;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        public async Task<int> ProcessPendingCasesAsync(CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            var pendingCases = await _context.Cases
                .Where(c => c.Status == CaseStatus.Pending)
                .ToListAsync(cancellationToken);

            int processedCount = 0;

            foreach (var caseItem in pendingCases)
            {
                Domain.Entities.BankCustomer? bankCustomer = null;

                if (!string.IsNullOrWhiteSpace(caseItem.AccountNumber))
                {
                    bankCustomer = await _context.BankCustomers
                        .FirstOrDefaultAsync(b => b.AccountNumber == caseItem.AccountNumber, cancellationToken);
                }

                if (bankCustomer == null && !string.IsNullOrWhiteSpace(caseItem.AadhaarNumber))
                {
                    bankCustomer = await _context.BankCustomers
                        .FirstOrDefaultAsync(b => b.AadhaarNumber == caseItem.AadhaarNumber, cancellationToken);
                }

                if (bankCustomer == null && !string.IsNullOrWhiteSpace(caseItem.PanNumber))
                {
                    bankCustomer = await _context.BankCustomers
                        .FirstOrDefaultAsync(b => b.PanNumber == caseItem.PanNumber, cancellationToken);
                }

                if (bankCustomer != null)
                {
                    caseItem.Status = CaseStatus.AccountValidated;
                    caseItem.MatchedAccountNumber = bankCustomer.AccountNumber;
                    caseItem.BatchFoundBalance = bankCustomer.CurrentBalance;
                    caseItem.BatchAccountStatus = bankCustomer.AccountStatus;
                    caseItem.SystemRemarks = "Account matched and validated successfully.";
                    
                    await _auditLogService.LogAsync("BatchValidation", "Case", "AccountValidated", caseItem.Id, cancellationToken);
                }
                else
                {
                    caseItem.Status = CaseStatus.AccountNotFound;
                    caseItem.SystemRemarks = "Account not found in bank records.";
                    
                    await _auditLogService.LogAsync("BatchValidation", "Case", "AccountNotFound", caseItem.Id, cancellationToken);
                }

                processedCount++;
            }

            if (processedCount > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            stopwatch.Stop();
            _logger.LogInformation("BatchValidationService processed {count} pending cases in {duration} ms.", processedCount, stopwatch.ElapsedMilliseconds);

            return processedCount;
        }
    }
}
