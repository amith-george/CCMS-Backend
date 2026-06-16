
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

        public async Task<PagedResult<CaseDto>> GetBankCasesAsync(int page = 1, int limit = 15, CancellationToken cancellationToken = default)
        {
            var query = _context.Cases.AsQueryable();
            var totalCount = await query.CountAsync(cancellationToken);
            var cases = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync(cancellationToken);

            return new PagedResult<CaseDto>
            {
                Data = cases.Select(MapToDto),
                TotalCount = totalCount,
                Page = page,
                Limit = limit
            };
        }

        public async Task<CaseStatisticsDto> GetBankStatisticsAsync(CancellationToken cancellationToken = default)
        {
            var stats = await _context.Cases
                .GroupBy(c => 1)
                .Select(g => new CaseStatisticsDto
                {
                    TotalCases = g.Count(),
                    PendingBatch = g.Count(c => c.Status == CaseStatus.Pending),
                    AwaitingAction = g.Count(c => c.Status == CaseStatus.AccountValidated),
                    AutoResolved = g.Count(c => c.Status == CaseStatus.AccountNotFound),
                    Completed = g.Count(c => c.Status == CaseStatus.FreezeApplied || c.Status == CaseStatus.BalanceProvided)
                })
                .FirstOrDefaultAsync(cancellationToken) ?? new CaseStatisticsDto();
                
            return stats;
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

            if (caseItem.OrderType == OrderType.FreezeAmount)
            {
                var bankCustomer = await _context.BankCustomers.FirstOrDefaultAsync(b => b.AccountNumber == caseItem.MatchedAccountNumber, cancellationToken);
                if (bankCustomer == null)
                {
                    throw new Exception("Associated bank account not found.");
                }

                if (responseDto.FinalFreezeAmount > bankCustomer.CurrentBalance)
                {
                    throw new Exception($"Insufficient account balance. The freeze amount cannot exceed the current balance of {bankCustomer.CurrentBalance}.");
                }

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
