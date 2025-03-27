using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Pizzashop_Project.Controllers;

public class CustomerController :Controller
{
    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public IActionResult Customers()
    {
        var customers = _customerService.GetAllCustomers();
        ViewData["sidebar-active"] = "Customer";
        return View(customers);
    }

        public IActionResult PaginatedCustomersData(string search, string sortColumn, string sortDirection, string timePeriod, string startDate, string endDate, int pageNumber = 1, int pageSize = 5)
    {

        var customers = _customerService.GetAllCustomers(search, sortColumn, sortDirection, pageNumber, pageSize, timePeriod, startDate, endDate);
        return PartialView("_CustomerListPartial", customers);
    }


        public async Task<IActionResult> ExportCustomerDataToExcel(string search = "", string status = "", string timePeriod = "", string startDate="", string endDate="")
    {
        var FileData = await _customerService.ExportCustomerData(search, status, timePeriod,startDate,endDate);
        var result = new FileContentResult(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "Customers.xlsx"
        };

        return result;
    }
}
