using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCMS.Application.DTOs;
using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;
using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CCMS.Infrastructure.Services
{
    public class CaseService : ICaseService
    {
        private readonly ApplicationDbContext _context;

        public CaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CaseDto> CreateCaseAsync(CreateCaseDto dto)
        {
            var today = DateTime.UtcNow.Date;
            
            // Get count of cases created today to generate sequence number
            var caseCountToday = await _context.Cases
                .Where(c => c.CreatedAt >= today)
                .CountAsync();

            // CCMS-YYYYMMDD-XXXX
            var caseNumber = $"CCMS-{today:yyyyMMdd}-{(caseCountToday + 1):D4}";

            var newCase = new Case
            {
                CaseNumber = caseNumber,
                DefendantName = dto.DefendantName,
                TargetBank = dto.TargetBank,
                AccountNumber = dto.AccountNumber,
                AadhaarNumber = dto.AadhaarNumber,
                PanNumber = dto.PanNumber,
                OrderType = dto.OrderType,
                RequestedFreezeAmount = dto.RequestedFreezeAmount,
                Status = CCMS.Domain.Enums.CaseStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Cases.Add(newCase);
            await _context.SaveChangesAsync();

            return MapToDto(newCase);
        }

        public async Task<IEnumerable<CaseDto>> GetCasesAsync()
        {
            var cases = await _context.Cases
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return cases.Select(MapToDto);
        }

        private static CaseDto MapToDto(Case c)
        {
            return new CaseDto
            {
                Id = c.Id,
                CaseNumber = c.CaseNumber,
                DefendantName = c.DefendantName,
                TargetBank = c.TargetBank,
                AccountNumber = c.AccountNumber,
                AadhaarNumber = c.AadhaarNumber,
                PanNumber = c.PanNumber,
                OrderType = c.OrderType,
                RequestedFreezeAmount = c.RequestedFreezeAmount,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                ResolvedAt = c.ResolvedAt,
                BankRemarks = c.BankRemarks,
                SystemRemarks = c.SystemRemarks
            };
        }
    }
}
