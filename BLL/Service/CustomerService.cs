using System.Drawing;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BLL.Service;

public class CustomerService : ICustomerService
{
    private readonly PizzashopDbContext _context;

    public CustomerService(PizzashopDbContext context)
    {
        _context = context;
    }

    public PaginationViewModel<CustomerViewModel> GetAllCustomers(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string timePeriod = "", string startDate = "", string endDate = "")
    {
        var query = _context.Customers
              .Include(x => x.Orders)
              .Where(x => x.Isdelete == false)
              .Select(x => new CustomerViewModel
              {
                  CustomerId = x.CustomerId,
                  CustomerName = x.CustomerName,
                  Phoneno = x.Phoneno,
                  Email = x.Email,
                  date = DateOnly.FromDateTime((DateTime)x.CreatedAt),
                  TotalOrders = x.Orders.Count().ToString()
              }).AsQueryable();


        //search
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();
            query = query.Where(u => u.CustomerName.ToLower().Contains(lowerSearchTerm) || u.Email.ToLower().Contains(lowerSearchTerm));
        }
        //sorting
        switch (sortColumn)
        {
            case "Name":
                query = sortDirection == "asc" ? query.OrderBy(u => u.CustomerName) : query.OrderByDescending(u => u.CustomerName);
                break;

            case "Date":
                query = sortDirection == "asc" ? query.OrderBy(u => u.date) : query.OrderByDescending(u => u.date);
                break;

            case "TotalOrder":
                query = sortDirection == "asc" ? query.OrderBy(u => u.TotalOrders) : query.OrderByDescending(u => u.TotalOrders);
                break;
        }

        //filter by time period
        switch (timePeriod)
        {
            case "All Time":
                query = query;
                break;
            case "7":
                query = query.Where(x => x.date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)));
                break;
            case "30":
                query = query.Where(x => x.date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)));
                break;
            case "Current Month":
                query = query.Where(x => x.date.Month == DateTime.Now.Month);
                break;
            case "custom Date":
                if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    query = query.Where(x => x.date >= DateOnly.FromDateTime(DateTime.Parse(startDate)) && x.date <= DateOnly.FromDateTime(DateTime.Now));
                }
                if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    query = query.Where(x => x.date <= DateOnly.FromDateTime(DateTime.Parse(endDate)));
                }
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    query = query.Where(x => x.date >= DateOnly.FromDateTime(DateTime.Parse(startDate)) && x.date <= DateOnly.FromDateTime(DateTime.Parse(endDate)));
                }
                break;
        }

       
        // Get total records count (before pagination)
        int totalCount = query.Count();

        // Apply pagination
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<CustomerViewModel>(items, totalCount, pageNumber, pageSize);
    }

    public Task<byte[]> ExportCustomerData(string search = "", string timePeriod = "", string startDate = "", string endDate = "")
    {
        var query = _context.Customers
              .Include(x => x.Orders)
              .Where(x => x.Isdelete == false)
              .Select(x => new CustomerViewModel
              {
                  CustomerId = x.CustomerId,
                  CustomerName = x.CustomerName,
                  Phoneno = x.Phoneno,
                  Email = x.Email,
                  date = DateOnly.FromDateTime((DateTime)x.CreatedAt),
                  TotalOrders = x.Orders.Count().ToString()
              }).AsQueryable();


        //search
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();
            query = query.Where(u => u.CustomerName.ToLower().Contains(lowerSearchTerm) || u.Email.ToLower().Contains(lowerSearchTerm));
        }

        //filter by time period
        switch (timePeriod)
        {
            case "All Time":
                query = query;
                break;
            case "7":
                query = query.Where(x => x.date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7)));
                break;
            case "30":
                query = query.Where(x => x.date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)));
                break;
            case "Current Month":
                query = query.Where(x => x.date.Month == DateTime.Now.Month);
                break;
            case "custom Date":
                if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    query = query.Where(x => x.date >= DateOnly.FromDateTime(DateTime.Parse(startDate)) && x.date <= DateOnly.FromDateTime(DateTime.Now));
                }
                if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    query = query.Where(x => x.date <= DateOnly.FromDateTime(DateTime.Parse(endDate)));
                }
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var demo = DateOnly.FromDateTime(DateTime.Parse(startDate));
                    query = query.Where(x => x.date >= DateOnly.FromDateTime(DateTime.Parse(startDate)) && x.date <= DateOnly.FromDateTime(DateTime.Parse(endDate)));
                }
                break;
        }


        var customers = query.ToList();

        // Create Excel package
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Customers");
            var currentRow = 3;
            var currentCol = 2;

            // this is first row....................................
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "Account: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


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
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = search;
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


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
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = timePeriod;
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 5;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "No. of Records: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = customers.Count;
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // this is table ....................................
            int headingRow = currentRow + 4;
            int headingCol = 2;

            worksheet.Cells[headingRow, headingCol].Value = "Id";
            headingCol++;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Name";
            headingCol += 3;  // Move to next unmerged column

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 3].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Email";
            headingCol += 4;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Date";
            headingCol += 3;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Mobile Number";
            headingCol += 3;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Total Order";
            headingCol += 2;

            using (var headingCells = worksheet.Cells[headingRow, 2, headingRow, headingCol - 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);

                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }


            // Populate data
            int row = headingRow + 1;

            foreach (var customer in customers)
            {
                int startCol = 2;

                worksheet.Cells[row, startCol].Value = customer.CustomerId;
                startCol += 1;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                worksheet.Cells[row, startCol].Value = customer.CustomerName;
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 3].Merge = true;
                worksheet.Cells[row, startCol].Value = customer.Email;
                startCol += 4;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                worksheet.Cells[row, startCol].Value = customer.date;
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                worksheet.Cells[row, startCol].Value = customer.Phoneno;
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
                worksheet.Cells[row, startCol].Value = customer.TotalOrders;
                startCol += 2;

                using (var rowCells = worksheet.Cells[row, 2, row, startCol - 1])
                {
                    // Apply black borders to each row
                    rowCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


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

    #region GetCustomerHistoryById
    public async Task<CustomerHistoryViewModel> GetCustomerHistoryById(long custid)
    {
        try
        {
            Customer? customerdetails = await _context.Customers.Include(x => x.Orders).ThenInclude(x => x.Orderdetails)
            .Include(x => x.Orders).ThenInclude(x => x.Paymentstatus)
            .FirstOrDefaultAsync(x => x.CustomerId == custid && x.Isdelete == false);
            if (customerdetails == null) return null;

            CustomerHistoryViewModel customerHistoryvm = new CustomerHistoryViewModel();

            customerHistoryvm.CustomerId = customerdetails.CustomerId;
            customerHistoryvm.CustomerName = customerdetails.CustomerName;
            customerHistoryvm.Phoneno = customerdetails.Phoneno;
            customerHistoryvm.ComingSince = (DateTime)customerdetails.CreatedAt;
            if (customerdetails.Orders.Count() == 0)
            {
                customerHistoryvm.AvgBill = 0;
                customerHistoryvm.MaxOrder = 0;
                customerHistoryvm.Visits = 0;
                customerHistoryvm.ordersList = null;
                return customerHistoryvm;
            }
            customerHistoryvm.AvgBill = customerdetails.Orders.Average(x => x.TotalAmount);
            customerHistoryvm.MaxOrder = customerdetails.Orders.Max(x => x.TotalAmount);
            customerHistoryvm.Visits = customerdetails.Orders.Count();
            customerHistoryvm.ordersList = customerdetails.Orders.Select(m => new OrderInCustomerForHistory
            {
                OrderId = m.OrderId,
                OrderDate = DateOnly.FromDateTime(m.OrderDate),
                OrderType = "DineIn",
                Payment = m.Paymentstatus.Paymentstatus,
                NoOfItems = m.Orderdetails.Count(),
                Amount = m.TotalAmount
            }).ToList();

            return customerHistoryvm;

        }
        catch (Exception e)
        {
            return null;
        }
    }

    #endregion

    #region GetCustomer which contains email
    public List<Customer> GetCustomerByEmail(string Email){
        List<Customer> customer = _context.Customers.Where(x => x.Email.Contains(Email.Trim()) && x.Isdelete==false).ToList();
        return customer ;
    }
    #endregion

    #region GetCustomerIdByTableId
    public async Task<long> GetCustomerIdByTableId(long tableId){
        Assigntable? customer =await _context.Assigntables.FirstOrDefaultAsync(x => x.TableId ==tableId && x.Isdelete == false);
        return customer !=null? customer.CustomerId : 0;
    }
    #endregion

}
