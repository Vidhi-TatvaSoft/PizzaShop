using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class OrderService : IOrderService
{
    private readonly PizzashopDbContext _context;

    public OrderService(PizzashopDbContext context)
    {
        _context = context;
    }

    public PaginationViewModel<OrderViewModel> GetAllOrders(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5,string status="",string timePeriod="",string startDate="",string endDate="")
    {
        var query = _context.Orders
              .Include(x => x.Customer)
              .Include(x => x.Paymentmethod)
              .Where(x=>x.Isdelete==false)
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
            query = query.Where(u => u.CustomerName.ToLower().Contains(lowerSearchTerm)
               
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
        switch(status)
        {
            case "All Status":
                query = query;
                break;
            case "Pending":
                query = query.Where(x => x.Status == "Pending");
                break;
            case "In Progress":
                query = query.Where(x => x.Status == "In Progress");
                break;
            case "Served":
                query = query.Where(x => x.Status == "Served");
                break;
            case "Completed":
                query = query.Where(x => x.Status == "Completed");
                break;
            case "Cancelled":
                query = query.Where(x => x.Status == "Cancelled");
                break;
            case "On Hold":
                query = query.Where(x => x.Status == "On Hold");
                break;
            case "Failed":
                query = query.Where(x => x.Status == "Failed");
                break;
        }

        //filter by time period
        switch(timePeriod)
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
        if(string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
        {
            query = query.Where(x => x.OrderDate <= DateOnly.FromDateTime(DateTime.Parse(endDate)));
        }




        // Get total records count (before pagination)
        int totalCount = query.Count();

        // Apply pagination
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<OrderViewModel>(items, totalCount, pageNumber, pageSize);
    }

    public PaginationViewModel<OrderViewModel> GetOrdersToExport(string search = "",string status="",string timePeriod=""){
        var query = _context.Orders
              .Include(x => x.Customer)
              .Include(x => x.Paymentmethod)
              .Where(x=>x.Isdelete==false)
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
            query = query.Where(u => u.CustomerName.ToLower().Contains(lowerSearchTerm)
               
            );
        }

        //filter by status
        switch(status)
        {
            case "All Status":
                query = query;
                break;
            case "Pending":
                query = query.Where(x => x.Status == "Pending");
                break;
            case "In Progress":
                query = query.Where(x => x.Status == "In Progress");
                break;
            case "Served":
                query = query.Where(x => x.Status == "Served");
                break;
            case "Completed":
                query = query.Where(x => x.Status == "Completed");
                break;
            case "Cancelled":
                query = query.Where(x => x.Status == "Cancelled");
                break;
            case "On Hold":
                query = query.Where(x => x.Status == "On Hold");
                break;
            case "Failed":
                query = query.Where(x => x.Status == "Failed");
                break;
        }

        //filter by time period
        switch(timePeriod)
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

        // Get total records count (before pagination)
        int totalCount = query.Count();

        // Apply pagination
        var items = query.ToList();

        return new PaginationViewModel<OrderViewModel>(items, totalCount, 1, 1);



    }


}
