using System.Drawing;
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

    // public IActionResult ExportOrders(string search, string status, string timePeriod)
    // {
    //     try
    //     {
    //         var orders = _orderService.GetOrdersToExport(search, status, timePeriod);

    //         // Create Excel package
    //         ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    //         using (var package = new ExcelPackage())
    //         {
    //             var worksheet = package.Workbook.Worksheets.Add("Orders");
    //             var currentRow = 3;
    //             var currentCol = 2;

    //             // this is first row....................................
    //             worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
    //             worksheet.Cells[currentRow, currentCol].Value = "Status: ";
    //             using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
    //                 headingCells.Style.Font.Bold = true;
    //                 headingCells.Style.Font.Color.SetColor(Color.White);
    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }
    //             currentCol += 2;
    //             worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
    //             worksheet.Cells[currentRow, currentCol].Value = status;
    //             using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }

    //             currentCol += 5;
    //             worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
    //             worksheet.Cells[currentRow, currentCol].Value = "Search Text: ";
    //             using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
    //                 headingCells.Style.Font.Bold = true;
    //                 headingCells.Style.Font.Color.SetColor(Color.White);
    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }

    //             currentCol += 2;
    //             worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
    //             worksheet.Cells[currentRow, currentCol].Value = search;
    //             using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }

    //             currentCol += 5;

    //             worksheet.Cells[currentRow, currentCol, currentRow + 4, currentCol + 1].Merge = true;

    //             // Insert Logo
    //             var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "pizzashop_logo.png");

    //             if (System.IO.File.Exists(imagePath))
    //             {
    //                 var picture = worksheet.Drawings.AddPicture("Image", new FileInfo(imagePath));
    //                 picture.SetPosition(currentRow - 1, 1, currentCol - 1, 1);
    //                 picture.SetSize(125, 95);
    //             }
    //             else
    //             {
    //                 worksheet.Cells[currentRow, currentCol].Value = "Image not found";
    //             }

    //             // this is second row....................................
    //             currentRow += 3;
    //             currentCol = 2;
    //             worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
    //             worksheet.Cells[currentRow, currentCol].Value = "Date: ";
    //             using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
    //                 headingCells.Style.Font.Bold = true;
    //                 headingCells.Style.Font.Color.SetColor(Color.White);
    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }

    //             currentCol += 2;
    //             worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
    //             worksheet.Cells[currentRow, currentCol].Value = timePeriod;
    //             using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }

    //             currentCol += 5;
    //             worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
    //             worksheet.Cells[currentRow, currentCol].Value = "No. of Records: ";
    //             using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
    //                 headingCells.Style.Font.Bold = true;
    //                 headingCells.Style.Font.Color.SetColor(Color.White);
    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }

    //             currentCol += 2;
    //             worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
    //             worksheet.Cells[currentRow, currentCol].Value = orders.Items.Count;
    //             using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }

    //             // this is table ....................................
    //             int headingRow = currentRow + 4;
    //             int headingCol = 2;

    //             worksheet.Cells[headingRow, headingCol].Value = "Order No";
    //             headingCol++;

    //             worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
    //             worksheet.Cells[headingRow, headingCol].Value = "Order Date";
    //             headingCol += 3;  // Move to next unmerged column

    //             worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
    //             worksheet.Cells[headingRow, headingCol].Value = "Customer Name";
    //             headingCol += 3;

    //             worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
    //             worksheet.Cells[headingRow, headingCol].Value = "Status";
    //             headingCol += 3;

    //             worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
    //             worksheet.Cells[headingRow, headingCol].Value = "Payment Mode";
    //             headingCol += 2;

    //             worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
    //             worksheet.Cells[headingRow, headingCol].Value = "Rating";
    //             headingCol += 2;

    //             worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
    //             worksheet.Cells[headingRow, headingCol].Value = "Total Amount";


    //             using (var headingCells = worksheet.Cells[headingRow, 2, headingRow, headingCol + 1])
    //             {
    //                 headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                 headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
    //                 headingCells.Style.Font.Bold = true;
    //                 headingCells.Style.Font.Color.SetColor(Color.White);

    //                 headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


    //                 headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                 headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //             }


    //             // Populate data
    //             int row = headingRow + 1;

    //             foreach (var order in orders.Items)
    //             {
    //                 int startCol = 2;

    //                 worksheet.Cells[row, startCol].Value = order.OrderId;
    //                 startCol += 1;

    //                 worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
    //                 worksheet.Cells[row, startCol].Value = order.OrderDate;
    //                 startCol += 3;

    //                 worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
    //                 worksheet.Cells[row, startCol].Value = order.CustomerName;
    //                 startCol += 3;

    //                 worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
    //                 worksheet.Cells[row, startCol].Value = order.Status;
    //                 startCol += 3;

    //                 worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
    //                 worksheet.Cells[row, startCol].Value = order.PaymentmethodName;
    //                 startCol += 2;

    //                 worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
    //                 worksheet.Cells[row, startCol].Value = order.Rating;
    //                 startCol += 2;

    //                 worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
    //                 worksheet.Cells[row, startCol].Value = order.TotalAmount;

    //                 using (var rowCells = worksheet.Cells[row, 2, row, startCol + 1])
    //                 {
    //                     // Apply alternating row colors (light gray for better readability)
    //                     if (row % 2 == 0)
    //                     {
    //                         rowCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
    //                         rowCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
    //                     }

    //                     // Apply black borders to each row
    //                     rowCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


    //                     rowCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
    //                     rowCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
    //                 }

    //                 row++;
    //             }
    //         }


    //         return Json(new { success = true, text = "Exported Successfully", data = orders });

    //     }
    //     catch (Exception ex)
    //     {
    //         return Json(new { success = false, message = ex.Message });
    //     }
    // }


    public async Task<IActionResult> ExportOrderDataToExcel(string search = "", string status = "", string timePeriod = "")
    {
        var FileData = await _orderService.ExportData(search, status, timePeriod);
        var result = new FileContentResult(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = "Orders.xlsx"
        };

        return result;
    }


    public IActionResult ViewOrderDetails(){
        return View();
    }

}
