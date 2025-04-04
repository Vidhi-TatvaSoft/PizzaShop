using System.Drawing;
using System.Reflection.Metadata;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iText.Layout;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using iText.Html2pdf;
using Rotativa.AspNetCore;
using Pizzashop_Project.Authorization;

namespace Pizzashop_Project.Controllers;

public class OrderController : Controller
{
    public readonly IOrderService _orderService;

    private readonly ICompositeViewEngine _ViewEngine;



    public OrderController(IOrderService orderService, ICompositeViewEngine viewEngine)
    {
        _orderService = orderService;
        _ViewEngine = viewEngine;
    }

    [PermissionAuthorize("Orders.View")]
    public IActionResult Orders()
    {
        var orders = _orderService.GetAllOrders();
        ViewData["sidebar-active"] = "Orders";
        return View(orders);
    }

    [PermissionAuthorize("Orders.View")]
    public IActionResult PaginatedOrdersData(string search, string sortColumn, string sortDirection, string Status, string timePeriod, string startDate, string endDate, int pageNumber = 1, int pageSize = 5)
    {

        var orders = _orderService.GetAllOrders(search, sortColumn, sortDirection, pageNumber, pageSize, Status, timePeriod, startDate, endDate);
        return PartialView("_OrderListPartial", orders);
    }

    [PermissionAuthorize("Orders.View")]
    public async Task<IActionResult> ExportOrderDataToExcel(string search = "", string status = "", string timePeriod = "")
    {
        var FileData = await _orderService.ExportData(search, status, timePeriod);
        var result = new FileContentResult(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "Orders.xlsx"
        };
        return result;
    }

    [PermissionAuthorize("Orders.View")]
    public IActionResult ViewOrderDetails(long orderid)
    {
        try{
        OrderDetaIlsInvoiceViewModel orderDetails = _orderService.GetOrderDetails(orderid);
        if(orderDetails == null)
        {
            TempData["ErrorMessage"] = $"Order not found for OrderId {orderid}.";
            return RedirectToAction("Orders");
        }
        return View(orderDetails);
        }catch(Exception ex)
        {
            // Handle the exception
            Console.WriteLine(ex.Message);
            TempData["ErrorMessage"] = "Something Went wrong";
            return RedirectToAction("Orders");
        }
    }

    [PermissionAuthorize("Orders.View")]
    public IActionResult GeneratePdfInvoice(long orderid)
    {
        OrderDetaIlsInvoiceViewModel orderDetails = _orderService.GetOrderDetails(orderid);
        if(orderDetails == null)
        {
            TempData["ErrorMessage"] = $"Order not found for OrderId {orderid}.";
            return RedirectToAction("Orders");
        }
        //   return PartialView("DownloadInvoicePdf", orderDetails);
        var generatedpdf = new ViewAsPdf("DownloadInvoicePdf", orderDetails)
        {
            FileName = "Invoice.pdf"
        };
        return generatedpdf;
    }

    [PermissionAuthorize("Orders.View")]
    public IActionResult GeneratePdfOrderDetails(long orderid)
    {
        OrderDetaIlsInvoiceViewModel orderDetails = _orderService.GetOrderDetails(orderid);
        if(orderDetails == null)
        {
            TempData["ErrorMessage"] = $"Order not found for OrderId {orderid}.";
            return RedirectToAction("Orders");
        }
        //   return PartialView("DownloadOrderDetailspdf", orderDetails);
        var generatedpdf = new ViewAsPdf("DownloadOrderDetailspdf", orderDetails)
        {
            FileName = "OrderDetails.pdf"
        };
        return generatedpdf;
    }

}
