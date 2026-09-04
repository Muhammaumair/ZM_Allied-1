using System.Threading.Tasks;
using ZMAllied.Application.DTOs.Dashboard;

namespace ZMAllied.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
