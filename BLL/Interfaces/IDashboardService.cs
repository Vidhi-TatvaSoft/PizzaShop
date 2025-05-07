using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IDashboardService
{
    public DashboardViewModel GetDashboardDetails(string timePeriod, string startDate, string endDate);
    public (List<decimal?>, List<int>) GetRevenueAndustomer(string timePeriod, string startDate, string endDate);
}
