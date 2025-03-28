using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderService
{
     public PaginationViewModel<OrderViewModel> GetAllOrders(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5,string status="",string timePeriod="",string startDate="",string endDate="");

      // public PaginationViewModel<OrderViewModel> GetOrdersToExport(string search = "",string status="",string timePeriod="");

      public Task<byte[]> ExportData(string search = "", string status = "", string timePeriod = "");

      public OrderDetaIlsInvoiceViewModel GetOrderDetails(long orderId);
}
