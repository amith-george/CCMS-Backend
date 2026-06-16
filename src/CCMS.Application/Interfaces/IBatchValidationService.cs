using System.Threading;
using System.Threading.Tasks;

namespace CCMS.Application.Interfaces
{
    public interface IBatchValidationService
    {
        Task<int> ProcessPendingCasesAsync(CancellationToken cancellationToken = default);
    }
}
