using BLL.Interfaces;
using DAL;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;

namespace BLL.Service;

public class DashboardService : IDashboardService
{
    private readonly PizzashopDbContext _context;

    public DashboardService(PizzashopDbContext context)
    {
        _context = context;
    }

    public DashboardViewModel GetDashboardDetails(string timePeriod, string startDate, string endDate)
    {
        try
        {

            DashboardViewModel dashboardvm = new();
            List<Order> query = _context.Orders.Where(o => !o.Isdelete && o.Status == "Completed").ToList();
            List<Item> itemList = _context.Items.Include(x => x.Orderdetails).Where(x => !x.Isdelete).OrderByDescending(x => x.Orderdetails.Count()).ToList();
            List<Waitinglist> waitinglist = _context.Waitinglists.Where(x => !x.Isdelete).ToList();
            List<Customer> customerList = _context.Customers.Where(c =>!c.Isdelete).ToList();
            DateOnly todate;
            DateOnly fromdate;
            DateOnly craetedAt;
            switch (timePeriod)
            {

                case "Today":
                    fromdate = DateOnly.FromDateTime(DateTime.Now);
                    todate = DateOnly.FromDateTime(DateTime.Now);
                    query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= todate).ToList();
                    itemList = itemList.Where(x => x.Orderdetails.Any(od => !od.Isdelete && DateOnly.FromDateTime((DateTime)od.CreatedAt!) >= todate)).ToList();
                    waitinglist = waitinglist.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= todate).ToList();
                    customerList = customerList.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= todate).ToList();
                    break;
                case "last7":
                    fromdate = DateOnly.FromDateTime(DateTime.Now);
                    todate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
                    query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7))).ToList();
                    itemList = itemList.Where(x => x.Orderdetails.Any(od => !od.Isdelete && DateOnly.FromDateTime((DateTime)od.CreatedAt!) >= todate)).ToList();
                    waitinglist = waitinglist.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7))).ToList();
                    customerList = customerList.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Now.AddDays(-7))).ToList();
                    break;
                case "last30":
                    fromdate = DateOnly.FromDateTime(DateTime.Now);
                    todate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
                    query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= todate).ToList();
                    itemList = itemList.Where(x => x.Orderdetails.Any(od => !od.Isdelete
                                            && DateOnly.FromDateTime((DateTime)od.CreatedAt!) >= todate)).ToList();
                    waitinglist = waitinglist.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= todate).ToList();
                    customerList = customerList.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= todate).ToList();

                    break;
                case "curentMonth":
                    // fromdate = DateOnly.FromDateTime(DateTime.Now).Month;
                    // todate = DateTime.Now.Month
                    query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!).Month == DateTime.Now.Month).ToList();
                    itemList = itemList.Where(x => x.Orderdetails.Any(od => !od.Isdelete
                                            && DateOnly.FromDateTime((DateTime)od.CreatedAt!).Month == DateTime.Now.Month)).ToList();
                    waitinglist = waitinglist.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!).Month == DateTime.Now.Month).ToList();
                    customerList = customerList.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!).Month == DateTime.Now.Month).ToList();
                    break;
                case "custom":
                    if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                    {
                        query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Parse(startDate))
                                            && DateOnly.FromDateTime((DateTime)x.CreatedAt) <= DateOnly.FromDateTime(DateTime.Now)).ToList();
                        itemList = itemList.Where(x => x.Orderdetails.Any(od => !od.Isdelete
                                            && DateOnly.FromDateTime((DateTime)od.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Parse(startDate))
                                            && DateOnly.FromDateTime((DateTime)od.CreatedAt) <= DateOnly.FromDateTime(DateTime.Now))).ToList();
                        waitinglist = waitinglist.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Parse(startDate))
                                            && DateOnly.FromDateTime((DateTime)x.CreatedAt) <= DateOnly.FromDateTime(DateTime.Now)).ToList();
                        customerList = customerList.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Parse(startDate))
                                            && DateOnly.FromDateTime((DateTime)x.CreatedAt) <= DateOnly.FromDateTime(DateTime.Now)).ToList();
                    }
                    if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                    {
                        query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) <= DateOnly.FromDateTime(DateTime.Parse(endDate))).ToList();
                        itemList = itemList.Where(x => x.Orderdetails.Any(od => !od.Isdelete
                                            && DateOnly.FromDateTime((DateTime)od.CreatedAt!) <= DateOnly.FromDateTime(DateTime.Parse(endDate)))).ToList();
                        waitinglist = waitinglist.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) <= DateOnly.FromDateTime(DateTime.Parse(endDate))).ToList();
                        customerList = customerList.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) <= DateOnly.FromDateTime(DateTime.Parse(endDate))).ToList();
                    }
                    if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                    {
                        query = query.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Parse(startDate))
                                            && DateOnly.FromDateTime((DateTime)x.CreatedAt) <= DateOnly.FromDateTime(DateTime.Parse(endDate))).ToList();
                        itemList = itemList.Where(x => x.Orderdetails.Any(od => !od.Isdelete
                                            && DateOnly.FromDateTime((DateTime)od.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Parse(startDate))
                                            && DateOnly.FromDateTime((DateTime)od.CreatedAt) <= DateOnly.FromDateTime(DateTime.Parse(endDate)))).ToList();
                        waitinglist = waitinglist.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Parse(startDate))
                                            && DateOnly.FromDateTime((DateTime)x.CreatedAt) <= DateOnly.FromDateTime(DateTime.Parse(endDate))).ToList();
                        customerList = customerList.Where(x => DateOnly.FromDateTime((DateTime)x.CreatedAt!) >= DateOnly.FromDateTime(DateTime.Parse(startDate))
                                            && DateOnly.FromDateTime((DateTime)x.CreatedAt) <= DateOnly.FromDateTime(DateTime.Parse(endDate))).ToList();                    
                    }
                    break;
            }
            dashboardvm.TotalSales = query.Where(o => !o.Isdelete).ToList().Sum(o => o.TotalAmount);
            dashboardvm.TotalOrders = query.Count(o => !o.Isdelete);
            if (dashboardvm.TotalOrders != 0)
                dashboardvm.AvgOrderAmount = Math.Round((decimal)dashboardvm.TotalSales / dashboardvm.TotalOrders, 2);
            else dashboardvm.AvgOrderAmount = 0;
            dashboardvm.DashboardItemList = new();
            dashboardvm.NewCustomerCount = customerList.Count();
            for (int i = 0; i < (itemList.Count > 4 ? 4 : itemList.Count); i++)
            {
                if (i < 2)
                {
                    dashboardvm.DashboardItemList.Add(new()
                    {
                        ItemImage = itemList[i].ItemImage!,
                        ItemName = itemList[i].ItemName,
                        NumberOfOrders = itemList[i].Orderdetails.Count
                    });
                }
                else
                {
                    dashboardvm.DashboardItemList.Add(new()
                    {
                        ItemImage = itemList[itemList.Count - (i - 1)].ItemImage!,
                        ItemName = itemList[itemList.Count - (i - 1)].ItemName,
                        NumberOfOrders = itemList[itemList.Count - (i - 1)].Orderdetails.Count
                    });
                }
            }
            if (itemList.Count < 4)
            {
                for (int i = itemList.Count; i < 4; i++)
                {
                    dashboardvm.DashboardItemList.Add(null!);
                }
            }
            dashboardvm.WaitingListCount = waitinglist.Where(x => !x.Isdelete && !x.Isassign).ToList().Count;
            List<TimeSpan?> dateTimes = waitinglist.Where(x => x.Isassign && !x.Isdelete).Select(x => x.AssignedAt - x.CreatedAt).ToList();
            if (waitinglist.Count != 0 && dateTimes.Count > 0)
            {
                int AverageMinutes = (int)dateTimes.Average(x => TimeSpan.Parse(x.ToString() != "" ? x.ToString()! : "0.00:00:00.0").Minutes);
                int AverageSeconds = (int)dateTimes.Average(x => TimeSpan.Parse(x.ToString() != "" ? x.ToString()! : "0.00:00:00.0").Seconds);
                dashboardvm.AvgWaitingTime = AverageMinutes.ToString() + "min(s) " + AverageSeconds.ToString() + "sec(s)";
            }
            else
            {
                dashboardvm.AvgWaitingTime = "0 min 0 sec";
            }
            return dashboardvm;

        }
        catch (Exception e)
        {
            return new DashboardViewModel();
        }
    }

    public (List<decimal?>, List<int>) GetRevenueAndustomer(string timePeriod, string startDate, string endDate)
    {
        List<Order> orders = _context.Orders.Where(o => !o.Isdelete && o.Status == "Completed").ToList();
        List<Customer> customers = _context.Customers.Where(c => !c.Isdelete).ToList();
        List<decimal?> RevenueList = new();
        List<int> CustomerList = new();
    
        switch (timePeriod)
        {
            case "Today":
                for (int i = 0; i <= 23; i++)
                {
                    CustomerList.Add(customers.Where(x => x.CreatedAt.Date == DateTime.Now.Date && x.CreatedAt.Hour == i).ToList().Count);
                    RevenueList.Add(orders.Where(x => x.CreatedAt.Date == DateTime.Now.Date && x.CreatedAt.Hour == i).Sum(x => x.TotalAmount));
                }
                break;
            case "last7":
                for (int i = -1 * 7; i <= 0; i++)
                {
                    CustomerList.Add(customers.Where(x => x.CreatedAt.Date == DateTime.Now.AddDays(i).Date).ToList().Count);
                    RevenueList.Add(orders.Where(x => x.CreatedAt.Date == DateTime.Now.AddDays(i).Date).Sum(x => x.TotalAmount));
                }

                break;
            case "last30":
                for (int i = -1 * 30; i <= 0; i++)
                {
                    CustomerList.Add(customers.Where(x => x.CreatedAt.Date == DateTime.Now.AddDays(i).Date).ToList().Count);
                    RevenueList.Add(orders.Where(x => x.CreatedAt.Date == DateTime.Now.AddDays(i).Date).Sum(x => x.TotalAmount));
                }
                break;
            case "curentMonth":
                for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
                {
                    CustomerList.Add(customers.Where(x => x.CreatedAt.Day == i && x.CreatedAt.Month == DateTime.Now.Month).ToList().Count);
                    RevenueList.Add(orders.Where(x => x.CreatedAt.Day == i && x.CreatedAt.Month == DateTime.Now.Month).Sum(x => x.TotalAmount));
                }
                break;
            case "custom":
                if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        CustomerList.Add(customers.Where(x => x.CreatedAt.Month == i && x.CreatedAt.Date >= DateTime.Parse(startDate).Date).ToList().Count);
                        RevenueList.Add(orders.Where(x => x.CreatedAt.Month == i && x.CreatedAt.Date >= DateTime.Parse(startDate).Date).Sum(x => x.TotalAmount));
                    }
                }
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        CustomerList.Add(customers.Where(x => x.CreatedAt.Month == i && x.CreatedAt.Date <= DateTime.Parse(endDate).Date).ToList().Count);
                        RevenueList.Add(orders.Where(x => x.CreatedAt.Month == i && x.CreatedAt.Date <= DateTime.Parse(endDate).Date).Sum(x => x.TotalAmount));
                    }
                }
                else
                {
                    string[] StartDateList = startDate.Split("-");
                    string[] EndDateList = endDate.Split("-");
                    if (StartDateList[1] == EndDateList[1])
                    {
                        for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
                        {
                            CustomerList.Add(customers.Where(x => x.CreatedAt.Day == i && x.CreatedAt.Date >= DateTime.Parse(startDate).Date 
                                            && x.CreatedAt.Date <= DateTime.Parse(endDate).Date).ToList().Count);
                            RevenueList.Add(orders.Where(x => x.CreatedAt.Day == i && x.CreatedAt.Date >= DateTime.Parse(startDate).Date 
                                            && x.CreatedAt.Date <= DateTime.Parse(endDate).Date).Sum(x => x.TotalAmount));
                        }
                    }
                    else
                    {
                        for (int i = int.Parse(StartDateList[1]); i <= int.Parse(EndDateList[1]); i++)
                        {
                            CustomerList.Add(customers.Where(x => x.CreatedAt.Month == i && x.CreatedAt.Date >= DateTime.Parse(startDate).Date 
                                            && x.CreatedAt.Date <= DateTime.Parse(endDate).Date).ToList().Count);
                            RevenueList.Add(orders.Where(x => x.CreatedAt.Month == i && x.CreatedAt.Date >= DateTime.Parse(startDate).Date 
                                            && x.CreatedAt.Date <= DateTime.Parse(endDate).Date).Sum(x => x.TotalAmount));
                        }
                    }
                }
                break;
        }
        return (RevenueList, CustomerList);
    }

}
