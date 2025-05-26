using System.Drawing;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BLL.Service;

public class OrderService : IOrderService
{
    private readonly PizzashopDbContext _context;

    public OrderService(PizzashopDbContext context)
    {
        _context = context;
    }

    #region Get al orders for pagination
    public PaginationViewModel<OrderViewModel> GetAllOrders(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string status = "", string timePeriod = "", string startDate = "", string endDate = "")
    {
        try
        {

            var query = _context.Orders
                  .Include(x => x.Customer)
                  .Include(x => x.Paymentmethod)
                  .Where(x => x.Isdelete == false)
                  .Select(x => new OrderViewModel
                  {
                      OrderId = x.OrderId,
                      CustomerId = x.CustomerId,
                      CustomerName = x.Customer.CustomerName,
                      OrderDate = DateOnly.FromDateTime(x.OrderDate),
                      Status = x.Status,
                      RatingId = x.RatingId == null ? 0 : x.RatingId,
                      Rating = x.RatingId == null ? 0 : (int)Math.Ceiling(((double)x.Rating.Food + (double)x.Rating.Service + (double)x.Rating.Ambience) / 3),
                      TotalAmount = x.TotalAmount,
                      PaymentmethodId = x.PaymentmethodId,
                      PaymentmethodName = x.Paymentmethod.Paymenttype
                  }).AsQueryable();

            //search
            if (!string.IsNullOrEmpty(search))
            {
                string lowerSearchTerm = search.ToLower();
                query = query.Where(u => u.CustomerName.ToLower().Contains(lowerSearchTerm) ||
                u.OrderId.ToString().Contains(lowerSearchTerm)
                );
            }
            //sorting
            switch (sortColumn)
            {
                case "OrderId":
                    query = sortDirection == "asc" ? query.OrderBy(u => u.OrderId) : query.OrderByDescending(u => u.OrderId);
                    break;

                case "Date":
                    query = sortDirection == "asc" ? query.OrderBy(u => u.OrderDate) : query.OrderByDescending(u => u.OrderDate);
                    break;

                case "Customer":
                    query = sortDirection == "asc" ? query.OrderBy(u => u.CustomerName) : query.OrderByDescending(u => u.CustomerName);
                    break;

                case "Amount":
                    query = sortDirection == "asc" ? query.OrderBy(u => u.TotalAmount) : query.OrderByDescending(u => u.TotalAmount);
                    break;
            }



            //filter by status
            if (!string.IsNullOrEmpty(status) && status != "All Status")
            {
                query = query.Where(x => x.Status == status);
            }

            //filter by time period
            switch (timePeriod)
            {
                case "All Time":
                    query = query;
                    break;
                case "7":
                    query = query.Where(x => x.OrderDate >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)));
                    break;
                case "30":
                    query = query.Where(x => x.OrderDate >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)));
                    break;
                case "Current Month":
                    query = query.Where(x => x.OrderDate.Month == DateTime.Now.Month);
                    break;
            }

            //filter by date
            if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
            {
                query = query.Where(x => x.OrderDate >= DateOnly.FromDateTime(DateTime.Parse(startDate)) && x.OrderDate <= DateOnly.FromDateTime(DateTime.Now));
            }
            if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                query = query.Where(x => x.OrderDate <= DateOnly.FromDateTime(DateTime.Parse(endDate)));
            }
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                query = query.Where(x => x.OrderDate >= DateOnly.FromDateTime(DateTime.Parse(startDate)) && x.OrderDate <= DateOnly.FromDateTime(DateTime.Parse(endDate)));
            }




            // Get total records count (before pagination)
            int totalCount = query.Count();

            // List<OrderViewModel> demoList = query.ToList();

            // Apply pagination
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PaginationViewModel<OrderViewModel>(items, totalCount, pageNumber, pageSize);

        }
        catch (Exception e)
        {
            return new PaginationViewModel<OrderViewModel>(null, 0, pageNumber, pageSize);
        }
    }
    #endregion

    #region Export data in excel
    public Task<byte[]> ExportData(string search = "", string status = "", string timePeriod = "")
    {
        var query = _context.Orders
              .Include(x => x.Customer)
              .Include(x => x.Paymentmethod)
              .Include(x => x.Rating)
              .Where(x => x.Isdelete == false)
              .Select(x => new OrderViewModel
              {
                  OrderId = x.OrderId,
                  CustomerId = x.CustomerId,
                  CustomerName = x.Customer.CustomerName,
                  OrderDate = DateOnly.FromDateTime(x.OrderDate),
                  Status = x.Status,
                  RatingId = x.RatingId,
                  Rating = (int)Math.Ceiling(((double)x.Rating.Food + (double)x.Rating.Service + (double)x.Rating.Ambience) / 3),
                  TotalAmount = x.TotalAmount,
                  PaymentmethodId = x.PaymentmethodId,
                  PaymentmethodName = x.Paymentmethod.Paymenttype
              }).AsQueryable();

        //search 
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();
            query = query.Where(u => u.CustomerName.ToLower().Contains(lowerSearchTerm) ||
            u.OrderId.ToString().Contains(lowerSearchTerm)
            );
        }

        //filter by status
        if (!string.IsNullOrEmpty(status) && status != "All Status")
        {
            query = query.Where(x => x.Status == status);
        }


        //filter by time period
        switch (timePeriod)
        {
            case "All Time":
                query = query;
                break;
            case "7":
                query = query.Where(x => x.OrderDate >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)));
                break;
            case "30":
                query = query.Where(x => x.OrderDate >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)));
                break;
            case "Current Month":
                query = query.Where(x => x.OrderDate.Month == DateTime.Now.Month);
                break;
        }

        var orders = query.ToList();

        // Create Excel package
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Orders");
            var currentRow = 3;
            var currentCol = 2;
            // var colorsforExcel = System.Drawing.Color;
            // this is first row....................................
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "Status: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(System.Drawing.Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = status;
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 5;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "Search Text: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(System.Drawing.Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = search;
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 5;

            worksheet.Cells[currentRow, currentCol, currentRow + 4, currentCol + 1].Merge = true;

            // Insert Logo
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "pizzashop_logo.png");

            if (File.Exists(imagePath))
            {
                var picture = worksheet.Drawings.AddPicture("Image", new FileInfo(imagePath));
                picture.SetPosition(currentRow - 1, 1, currentCol - 1, 1);
                picture.SetSize(125, 95);
            }
            else
            {
                worksheet.Cells[currentRow, currentCol].Value = "Image not found";
            }

            // this is second row....................................
            currentRow += 3;
            currentCol = 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "Date: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(System.Drawing.Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = timePeriod;
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 5;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "No. of Orders: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(System.Drawing.Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = orders.Count;
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // this is table ....................................
            int headingRow = currentRow + 4;
            int headingCol = 2;

            worksheet.Cells[headingRow, headingCol].Value = "Id";
            headingCol++;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Date";
            headingCol += 3;  // Move to next unmerged column

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Customer";
            headingCol += 3;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Status";
            headingCol += 3;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Payment Mode";
            headingCol += 2;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Rating";
            headingCol += 2;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Total Amount";


            using (var headingCells = worksheet.Cells[headingRow, 2, headingRow, headingCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(System.Drawing.Color.White);

                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }


            // Populate data
            int row = headingRow + 1;

            foreach (var order in orders)
            {
                int startCol = 2;

                worksheet.Cells[row, startCol].Value = order.OrderId;
                startCol += 1;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                worksheet.Cells[row, startCol].Value = order.OrderDate;
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                worksheet.Cells[row, startCol].Value = order.CustomerName;
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                worksheet.Cells[row, startCol].Value = order.Status;
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
                worksheet.Cells[row, startCol].Value = order.PaymentmethodName;
                startCol += 2;

                worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
                worksheet.Cells[row, startCol].Value = order.Rating;
                startCol += 2;

                worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
                worksheet.Cells[row, startCol].Value = order.TotalAmount;

                using (var rowCells = worksheet.Cells[row, 2, row, startCol + 1])
                {

                    // Apply black borders to each row
                    rowCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.Black);


                    rowCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                row++;
            }

            //  It creates a Task that is already completed and contains the specified result 
            // (in this case, the byte array).
            // This is useful when you need to return a Task in an asynchronous method but already have 
            // the result available synchronously.
            return Task.FromResult(package.GetAsByteArray());

        }

    }
    #endregion

    #region Get order details from orderId
    public OrderDetaIlsInvoiceViewModel GetOrderDetails(long orderId)
    {
        try
        {
            var orderdetails = _context.Invoices.Include(x => x.Order).ThenInclude(x => x.Table).ThenInclude(x => x.Section).Include(x => x.Customer)
            .Where(x => x.OrderId == orderId).FirstOrDefault();

            if (orderdetails == null)
            {
                return null;
            }

            OrderDetaIlsInvoiceViewModel orderdetailsvm = new();
            //order
            orderdetailsvm.OrderId = orderdetails.OrderId;
            orderdetailsvm.OrderDate = orderdetails.Order.OrderDate;
            orderdetailsvm.OrderStatus = orderdetails.Order.Status;
            orderdetailsvm.InvoiceId = orderdetails.InvoiceId;
            orderdetailsvm.InvoiceNo = orderdetails.InvoiceNo;


            //customer
            orderdetailsvm.CustomerId = orderdetails.Order.CustomerId;
            orderdetailsvm.CustomerName = orderdetails.Order.Customer.CustomerName;
            orderdetailsvm.Phoneno = orderdetails.Order.Customer.Phoneno;
            orderdetailsvm.Email = orderdetails.Order.Customer.Email;

            List<Assigntable> AssignTableList = _context.Assigntables.Include(x => x.Customer).Include(x => x.Order)
            .Where(x => x.CustomerId == orderdetails.Order.CustomerId && x.OrderId == orderId).ToList();
            orderdetailsvm.NumberOfPerson = AssignTableList.Sum(x => x.NoOfPerson);

            //table

            orderdetailsvm.tableList = _context.Assigntables.Include(x => x.Table)
            .Where(x => x.CustomerId == orderdetails.Order.CustomerId && x.OrderId == orderId)
            .Select(x => new Table
            {
                TableId = x.Order.Table.TableId,
                TableName = x.Order.Table.TableName

            }).ToList();
            orderdetailsvm.SectionId = orderdetails.Order.SectionId;
            orderdetailsvm.SectionName = orderdetails.Order.Section.SectionName;

            //items
            orderdetailsvm.ItemsInOrderDetails = _context.Orderdetails.Include(x => x.Item).Where(x => x.OrderId == orderId && !x.Isdelete)
            .Select(x => new ItemForInvoiceOrderDetails
            {
                ItemId = x.ItemId,
                ItemName = x.Item.ItemName,
                Quantity = x.Quantity,
                Rate = x.Item.Rate,
                TotalOfItemByQuantity = Math.Round(x.Quantity * x.Item.Rate, 2),
                ModifiersInItemInvoice = _context.Modifierorders.Include(m => m.Modifier).Include(m => m.Orderdetail).ThenInclude(m => m.Item)
                .Where(m => m.Orderdetail.OrderdetailId == x.OrderdetailId && !m.Isdelete)
                .Select(m => new ModifiersForItemInInvoiceOrderDetails
                {
                    ModifierId = m.ModifierId,
                    ModifierName = m.Modifier.ModifierName,
                    Rate = m.Modifier.Rate,
                    Quantity = x.Quantity,
                    TotalOfModifierByQuantity = Math.Round(x.Quantity * (decimal)m.Modifier.Rate, 2),
                }).ToList()
            }).ToList();
            if (orderdetails.Order.Status == "Cancelled")
            {
                orderdetailsvm.SubTotalAmountOfOrder = Math.Round((decimal)0,2);
                orderdetailsvm.TotalAmountOfOrderMain =  Math.Round((decimal)0,2);
            }
            else
            {
                orderdetailsvm.SubTotalAmountOfOrder = Math.Round((decimal)orderdetailsvm.ItemsInOrderDetails
                .Sum(x => x.TotalOfItemByQuantity + x.ModifiersInItemInvoice.Sum(x => x.TotalOfModifierByQuantity)), 2);
            }

            //taxes
            var taxedetails = _context.Taxinvoicemappings.Include(x => x.Invoice).Include(x => x.Tax)
            .Where(x => x.Invoice.OrderId == orderId).ToList();

            orderdetailsvm.TaxesInOrderDetails = new List<TaxForOrderDetailsInvoice>();
            foreach (var tax in taxedetails)
            {
                orderdetailsvm.TaxesInOrderDetails.Add(
                        new TaxForOrderDetailsInvoice
                        {
                            // TaxId = tax.Tax.TaxId,
                            TaxName = tax.TaxName,
                            // TaxType = tax.Tax.TaxType,
                            TaxValue = tax.TaxAmount
                        }
                    );
                // if (tax.Tax.TaxType == "Fix Amount")
                // {
                //     orderdetailsvm.TaxesInOrderDetails.Add(
                //         new TaxForOrderDetailsInvoice
                //         {
                //             TaxId = tax.Tax.TaxId,
                //             TaxName = tax.TaxName,
                //             TaxType = tax.Tax.TaxType,
                //             TaxValue = tax.Tax.TaxValue
                //         }
                //     );
                // }
                // else
                // {
                //     orderdetailsvm.TaxesInOrderDetails.Add(
                //         new TaxForOrderDetailsInvoice
                //         {
                //             TaxId = tax.Tax.TaxId,
                //             TaxName = tax.Tax.TaxName,
                //             TaxType = tax.Tax.TaxType,
                //             TaxValue = Math.Round(tax.Tax.TaxValue / 100 * orderdetailsvm.SubTotalAmountOfOrder, 2)
                //         }
                //     );
                // }
            }
            if (orderdetails.Order.Status != "Cancelled")
            {
                orderdetailsvm.TotalAmountOfOrderMain = orderdetailsvm.SubTotalAmountOfOrder + orderdetailsvm.TaxesInOrderDetails.Sum(x => x.TaxValue);
            }

            return orderdetailsvm;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
    #endregion


    #region  convert to pdf invoice
    public byte[] Compose()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        using (var stream = new MemoryStream())
        {
            Document.Create(container =>
            {
                container
                    .Page(page =>
                    {
                        page.Margin(50);

                        page.Header().Element(ComposeHeader);
                        page.Content().Background(Colors.Grey.Lighten3);
                        page.Footer().Height(50).Background(Colors.Grey.Lighten1);
                    });
            }).GeneratePdf(stream);

            return stream.ToArray();
        }
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.ConstantItem(150);
            row.ConstantItem(100).Height(50).Image(File.ReadAllBytes("../Pizzashop_Project/wwwroot/images/logos/pizzashop_logo.png"));
            row.RelativeItem().AlignMiddle().Column(column =>
            {
                column.Item()
                    .Text("PIZZASHOP")
                    .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
            });


        });
    }

    #endregion
}
