using DAL.ViewModels;

namespace BLL.Interfaces;

public interface ICustomerService
{
    public PaginationViewModel<CustomerViewModel> GetAllCustomers(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string timePeriod = "", string startDate = "", string endDate = "");
    public Task<byte[]> ExportCustomerData(string search = "",  string timePeriod = "",string startDate = "", string endDate = "");

    public Task<CustomerHistoryViewModel> GetCustomerHistoryById(long custid);
}
