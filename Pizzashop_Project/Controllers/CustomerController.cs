using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;
    [PermissionAuthorize("AdminManager")]


public class CustomerController :Controller
{
    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [PermissionAuthorize("Customers.View")]
    public IActionResult Customers()
    {
        var customers = _customerService.GetAllCustomers();
        ViewData["sidebar-active"] = "Customer";
        return View(customers);
    }

    [PermissionAuthorize("Customers.View")]
        public IActionResult PaginatedCustomersData(string search, string sortColumn, string sortDirection, string timePeriod, string startDate, string endDate, int pageNumber = 1, int pageSize = 5)
    {

        var customers = _customerService.GetAllCustomers(search, sortColumn, sortDirection, pageNumber, pageSize, timePeriod, startDate, endDate);
        return PartialView("_CustomerListPartial", customers);
    }

    [PermissionAuthorize("Customers.View")]
    public async Task<IActionResult> ExportCustomerDataToExcel(string search = "", string timePeriod = "", string startDate="", string endDate="")
    {
        var FileData = await _customerService.ExportCustomerData(search, timePeriod,startDate,endDate);
        var result = new FileContentResult(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "Customers.xlsx"
        };
        return result;
    }

    [PermissionAuthorize("Customers.View")]
    #region shoe customer history modal
    public async Task<IActionResult> ShowCustomerHistoryModal(long custid){
        CustomerHistoryViewModel customerHistoryvm =await _customerService.GetCustomerHistoryById(custid);
        return PartialView("_CustomerHistoryPartial" , customerHistoryvm);
    }
    #endregion
}
