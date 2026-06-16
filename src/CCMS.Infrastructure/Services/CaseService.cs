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
        private readonly IAuditLogService _auditLogService;
        private readonly IFileStorageService _fileStorageService;

        public CaseService(ApplicationDbContext context, IAuditLogService auditLogService, IFileStorageService fileStorageService)
        {
            _context = context;
            _auditLogService = auditLogService;
            _fileStorageService = fileStorageService;
        }

        public async Task<CaseDto> CreateCaseAsync(CreateCaseDto dto, 
            (System.IO.Stream Stream, string FileName, string ContentType) courtOrder,
            (System.IO.Stream Stream, string FileName, string ContentType) aadhaarDoc,
            (System.IO.Stream Stream, string FileName, string ContentType) panDoc)
        {
            var today = DateTime.UtcNow.Date;
            
            // Get count of cases created today to generate sequence number
            var caseCountToday = await _context.Cases
                .Where(c => c.CreatedAt >= today)
                .CountAsync();

            // CCMS-YYYYMMDD-XXXX
            var caseNumber = $"CCMS-{today:yyyyMMdd}-{(caseCountToday + 1):D4}";

            // Upload files first to ensure they are valid
            var courtOrderPath = await _fileStorageService.UploadFileAsync(courtOrder.Stream, courtOrder.FileName, courtOrder.ContentType);
            var aadhaarPath = await _fileStorageService.UploadFileAsync(aadhaarDoc.Stream, aadhaarDoc.FileName, aadhaarDoc.ContentType);
            var panPath = await _fileStorageService.UploadFileAsync(panDoc.Stream, panDoc.FileName, panDoc.ContentType);

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
                CreatedAt = DateTime.UtcNow,
                Documents = new List<CaseDocument>
                {
                    new CaseDocument { Type = CCMS.Domain.Enums.DocumentType.CourtOrder, FilePath = courtOrderPath, FileName = courtOrder.FileName },
                    new CaseDocument { Type = CCMS.Domain.Enums.DocumentType.Aadhaar, FilePath = aadhaarPath, FileName = aadhaarDoc.FileName },
                    new CaseDocument { Type = CCMS.Domain.Enums.DocumentType.PAN, FilePath = panPath, FileName = panDoc.FileName }
                }
            };

            _context.Cases.Add(newCase);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync("Create", "Case", $"Court Case {newCase.CaseNumber} successfully created for defendant {newCase.DefendantName}.", newCase.Id);

            return MapToDto(newCase);
        }

        public async Task<IEnumerable<CaseDto>> GetCasesAsync()
        {
            var cases = await _context.Cases
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return cases.Select(MapToDto);
        }

        public async Task<CaseDetailsDto?> GetCaseByIdAsync(int id)
        {
            var c = await _context.Cases
                .Include(x => x.Documents)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null) return null;

            var dto = new CaseDetailsDto
            {
                Id = c.Id,
                CaseNumber = c.CaseNumber,
                DefendantName = c.DefendantName,
                TargetBank = c.TargetBank,
                AccountNumber = CCMS.Application.Helpers.DataMasker.MaskAccountNumber(c.AccountNumber),
                AadhaarNumber = CCMS.Application.Helpers.DataMasker.MaskAadhaar(c.AadhaarNumber),
                PanNumber = CCMS.Application.Helpers.DataMasker.MaskPan(c.PanNumber),
                OrderType = c.OrderType,
                RequestedFreezeAmount = c.RequestedFreezeAmount,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                ResolvedAt = c.ResolvedAt,
                BankRemarks = c.BankRemarks,
                SystemRemarks = c.SystemRemarks,
                BatchAccountStatus = c.BatchAccountStatus,
                Documents = c.Documents.Select(d => new CaseDocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    DocumentType = d.Type.ToString(),
                    ContentType = GetContentType(d.FileName)
                }).ToList()
            };

            return dto;
        }

        public async Task<(System.IO.Stream? Stream, string ContentType, string FileName)> GetDocumentAsync(int caseId, int documentId)
        {
            var doc = await _context.CaseDocuments
                .FirstOrDefaultAsync(d => d.Id == documentId && d.CaseId == caseId);

            if (doc == null) return (null, string.Empty, string.Empty);

            var stream = await _fileStorageService.GetFileAsync(doc.FilePath);
            return (stream, GetContentType(doc.FileName), doc.FileName);
        }

        private static string GetContentType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "application/octet-stream";
            var ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream",
            };
        }

        private static CaseDto MapToDto(Case c)
        {
            return new CaseDto
            {
                Id = c.Id,
                CaseNumber = c.CaseNumber,
                DefendantName = c.DefendantName,
                TargetBank = c.TargetBank,
                AccountNumber = CCMS.Application.Helpers.DataMasker.MaskAccountNumber(c.AccountNumber),
                AadhaarNumber = CCMS.Application.Helpers.DataMasker.MaskAadhaar(c.AadhaarNumber),
                PanNumber = CCMS.Application.Helpers.DataMasker.MaskPan(c.PanNumber),
                OrderType = c.OrderType,
                RequestedFreezeAmount = c.RequestedFreezeAmount,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                ResolvedAt = c.ResolvedAt,
                BankRemarks = c.BankRemarks,
                SystemRemarks = c.SystemRemarks,
                BatchAccountStatus = c.BatchAccountStatus
            };
        }

    }
}
