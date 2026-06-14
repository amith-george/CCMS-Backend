using System;
using System.Threading;
using System.Threading.Tasks;
using CCMS.Application.Interfaces;
using CCMS.Domain.Entities;
using CCMS.Infrastructure.Persistence;

namespace CCMS.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AuditLogService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task LogAsync(string operation, string entityName, string action, int? caseId = null, CancellationToken cancellationToken = default)
        {
            var auditLog = new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                Operation = operation,
                EntityName = entityName,
                Action = action,
                UserId = _currentUserService.UserId,
                CaseId = caseId
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
