using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.DTOs;
using CCMS.Application.Interfaces;
using CCMS.Domain.Enums;
using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Services
{
    public class BankService : IBankService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditLogService _auditLogService;

        public BankService(ApplicationDbContext context, IAuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<CaseDto>> GetBankCasesAsync(CancellationToken cancellationToken = default)
        {
            var cases = await _context.Cases
                // .Include(c => c.Documents) // Omitted Include unless requested
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            return cases.Select(MapToDto);
        }

        public async Task<CaseDto?> GetBankCaseByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var caseItem = await _context.Cases
                // .Include(c => c.Documents)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (caseItem == null) return null;

            return MapToDto(caseItem);
        }

        public async Task<bool> SubmitBankResponseAsync(int caseId, BankResponseDto responseDto, CancellationToken cancellationToken = default)
        {
            var caseItem = await _context.Cases.FirstOrDefaultAsync(c => c.Id == caseId, cancellationToken);

            if (caseItem == null)
            {
                throw new Exception("Case not found.");
            }

            if (caseItem.Status != CaseStatus.AccountValidated)
            {
                throw new Exception("Case is not in a valid state to receive a bank response.");
            }

            if (caseItem.OrderType == OrderType.FreezeAccount)
            {
                caseItem.Status = CaseStatus.FreezeApplied;
                caseItem.FinalFreezeAmount = responseDto.FinalFreezeAmount;
                caseItem.SystemRemarks = "Bank applied freeze.";
                await _auditLogService.LogAsync("BankResponse", "Case", "FreezeApplied", caseItem.Id, cancellationToken);
            }
            else if (caseItem.OrderType == OrderType.BalanceEnquiry)
            {
                caseItem.Status = CaseStatus.BalanceProvided;
                caseItem.FinalReportedBalance = responseDto.FinalReportedBalance;
                caseItem.SystemRemarks = "Bank provided balance.";
                await _auditLogService.LogAsync("BankResponse", "Case", "BalanceProvided", caseItem.Id, cancellationToken);
            }

            caseItem.BankRemarks = responseDto.BankRemarks;
            caseItem.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static CaseDto MapToDto(Domain.Entities.Case caseItem)
        {
            return new CaseDto
            {
                Id = caseItem.Id,
                CaseNumber = caseItem.CaseNumber,
                DefendantName = caseItem.DefendantName,
                TargetBank = caseItem.TargetBank,
                AccountNumber = caseItem.AccountNumber,
                AadhaarNumber = caseItem.AadhaarNumber,
                PanNumber = caseItem.PanNumber,
                OrderType = caseItem.OrderType,
                RequestedFreezeAmount = caseItem.RequestedFreezeAmount,
                Status = caseItem.Status,
                CreatedAt = caseItem.CreatedAt,
                ResolvedAt = caseItem.ResolvedAt,
                BankRemarks = caseItem.BankRemarks,
                SystemRemarks = caseItem.SystemRemarks,
                MatchedAccountNumber = caseItem.MatchedAccountNumber,
                BatchFoundBalance = caseItem.BatchFoundBalance,
                FinalFreezeAmount = caseItem.FinalFreezeAmount,
                FinalReportedBalance = caseItem.FinalReportedBalance
            };
        }
    }
}
