using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IDashboardService
{
    public DashboardViewModel GetDashboardDetails();
}
