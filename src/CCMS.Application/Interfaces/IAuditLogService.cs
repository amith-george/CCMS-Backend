using System.Threading;
using System.Threading.Tasks;

namespace CCMS.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(string operation, string entityName, string action, int? caseId = null, CancellationToken cancellationToken = default);
    }
}
