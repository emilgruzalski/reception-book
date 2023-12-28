using Shared.DataTransferObjects;

namespace Service.Contracts;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}
