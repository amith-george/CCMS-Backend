using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.Interfaces;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Services
{
    public class BatchValidationService : IBatchValidationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public BatchValidationService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<int> ProcessPendingCasesAsync(CancellationToken cancellationToken = default)
        {
            var pendingCases = await _context.Cases
                .Where(c => c.Status == CaseStatus.Pending)
                .ToListAsync(cancellationToken);

            int processedCount = 0;

            foreach (var caseItem in pendingCases)
            {
                if (string.IsNullOrWhiteSpace(caseItem.AccountNumber))
                {
                    caseItem.Status = CaseStatus.AccountNotFound;
                    caseItem.SystemRemarks = "No account number provided.";
                }
                else
                {
                    // Look up the unique bank customer by AccountNumber
                    var bankCustomer = await _context.BankCustomers
                        .FirstOrDefaultAsync(b => b.AccountNumber == caseItem.AccountNumber, cancellationToken);

                    if (bankCustomer != null)
                    {
                        caseItem.Status = CaseStatus.AccountValidated;
                        caseItem.MatchedAccountNumber = bankCustomer.AccountNumber;
                        caseItem.BatchFoundBalance = bankCustomer.CurrentBalance;
                        caseItem.SystemRemarks = "Account matched and validated successfully.";
                        
                        await _auditLogService.LogAsync("BatchValidation", "Case", "AccountValidated", caseItem.Id, cancellationToken);
                    }
                    else
                    {
                        caseItem.Status = CaseStatus.AccountNotFound;
                        caseItem.SystemRemarks = "Account not found in bank records.";
                        
                        await _auditLogService.LogAsync("BatchValidation", "Case", "AccountNotFound", caseItem.Id, cancellationToken);
                    }
                }

                processedCount++;
            }

            if (processedCount > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return processedCount;
        }
    }
}
