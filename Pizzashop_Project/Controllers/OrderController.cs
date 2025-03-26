using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Pizzashop_Project.Controllers;

public class OrderController : Controller
{
    public readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    public IActionResult Orders()
    {
        var orders = _orderService.GetAllOrders();
        ViewData["sidebar-active"] = "Orders";
        return View(orders);
    }

    public IActionResult PaginatedOrdersData(string search, string sortColumn, string sortDirection, string Status, string timePeriod, string startDate, string endDate, int pageNumber = 1, int pageSize = 5)
    {

        var orders = _orderService.GetAllOrders(search, sortColumn, sortDirection, pageNumber, pageSize, Status, timePeriod, startDate, endDate);
        return PartialView("_OrderListPartial", orders);
    }

    public IActionResult ExportOrders(string search, string status, string timePeriod)
    {
        try
        {
            var orders = _orderService.GetOrdersToExport(search, status, timePeriod);

            // Export to Excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var pkg = new ExcelPackage())
            {

                var sheet = pkg.Workbook.Worksheets.Add("Expense");
                sheet.Cells[2,1,3,2].Merge = true;
                sheet.Cells[2,1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[2,1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[2,1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[2,1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[2,1].Value = "Status:";
                sheet.Cells[2,3,3,6].Merge = true;
                sheet.Cells[2,3,3,6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[2,3].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[2,3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[2,3].Value = "";

                sheet.Cells[2,8,3,9].Merge = true;
                sheet.Cells[2,8].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[2,8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[2,8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[2,8].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[2,8].Value = "Search Text:";
                sheet.Cells[2,10,3,13].Merge = true;
                sheet.Cells[2,10,3,13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[2,10].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[2,10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[2,10].Value = "";

                sheet.Cells[5,1,6,2].Merge = true;
                sheet.Cells[5,1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[5,1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[5,1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[5,1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[5,1].Value = "Date:";
                sheet.Cells[5,3,6,6].Merge = true;
                sheet.Cells[5,3,6,6].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[5,3].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[5,3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[5,3].Value = "";

                sheet.Cells[5,8,6,9].Merge = true;
                sheet.Cells[5,8].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[5,8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[5,8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[5,8].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[5,8].Value = "No. of records:";
                sheet.Cells[5,10,6,13].Merge = true;
                sheet.Cells[5,10,6,13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[5,10].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[5,10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[5,10].Value = "";

                sheet.Cells[9,1].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[9,1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[9,1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[9,1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[9,1].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[9,1].Value = "Id";

                sheet.Cells[9,2,9,4].Merge = true;
                sheet.Cells[9,2].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[9,2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[9,2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[9,2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[9,2].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[9,2].Value = "Date";

                sheet.Cells[9,5,9,7].Merge = true;
                sheet.Cells[9,5].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[9,5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[9,5].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[9,5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[9,5].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[9,5].Value = "Customer";

                sheet.Cells[9,8,9,10].Merge = true;
                sheet.Cells[9,8].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[9,8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[9,8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[9,8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[9,8].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[9,8].Value = "Status";

                sheet.Cells[9,11,9,12].Merge = true;
                sheet.Cells[9,11].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[9,11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[9,11].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[9,11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[9,11].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[9,11].Value = "Payment mode";

                sheet.Cells[9,13,9,14].Merge = true;
                sheet.Cells[9,13].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[9,13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[9,13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[9,13].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[9,13].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[9,13].Value = "Rating";

                sheet.Cells[9,15,9,16].Merge = true;
                sheet.Cells[9,15].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                sheet.Cells[9,15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[9,15].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[9,15].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                sheet.Cells[9,15].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#0d6efd"));
                sheet.Cells[9,15].Value = "Total amount";


                for (int i = 0; i < orders.Items.Count; i++)
                {
                    sheet.Cells[i + 10, 1].Value = orders.Items[i].OrderId;
                    sheet.Cells[i + 10, 2].Value = orders.Items[i].OrderDate;
                    sheet.Cells[i + 10, 5].Value = orders.Items[i].CustomerName;
                    sheet.Cells[i + 10, 8].Value = orders.Items[i].Status;
                    sheet.Cells[i + 10, 11].Value = orders.Items[i].PaymentmethodName;
                    sheet.Cells[i + 10, 13].Value = orders.Items[i].Rating;
                    sheet.Cells[i + 10, 15].Value = orders.Items[i].TotalAmount;
                }

                string p_strPath = DateTime.Now.Hour.ToString() + "_OrdersExport.xlsx";

                // Write content to excel file  
                System.IO.File.WriteAllBytes(p_strPath, pkg.GetAsByteArray());
                Console.WriteLine("Excel file created successfully with name: " + p_strPath);
                //Close Excel package 
                // pkg.Dispose(); 
                // Console.ReadKey(); 
            }


            return Json(new { success = true, text = "Exported Successfully", data = orders });

        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
