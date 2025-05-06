using BLL.Interfaces;
using DAL;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class DashboardService : IDashboardService
{
    private readonly PizzashopDbContext _context;

    public DashboardService(PizzashopDbContext context)
    {
        _context = context;
    }

    public DashboardViewModel GetDashboardDetails()
    {
        DashboardViewModel dashboardvm = new();
        dashboardvm.TotalSales = _context.Orders.Where(o => !o.Isdelete).ToList().Sum(o => o.TotalAmount);
        dashboardvm.TotalOrders = _context.Orders.Count(o => !o.Isdelete);
        dashboardvm.AverageOrderAmount = Math.Round((decimal)dashboardvm.TotalSales / dashboardvm.TotalOrders, 2);
        List<Item> itemList = _context.Items.Include(x => x.Orderdetails).OrderByDescending(x => x.Orderdetails.Count).ToList();
        dashboardvm.DashboardItemList = new();
        for (int i = 0; i < 4; i++)
        {
            if (i < 2)
            {
                dashboardvm.DashboardItemList.Add(new(){
                    ItemImage = itemList[i].ItemImage,
                    ItemName = itemList[i].ItemName,
                    NumberOfOrders = itemList[i].Orderdetails.Count
                });
            }else{
                dashboardvm.DashboardItemList.Add(new(){
                    ItemImage = itemList[itemList.Count - (i-1)].ItemImage,
                    ItemName = itemList[itemList.Count - (i-1)].ItemName,
                    NumberOfOrders = itemList[itemList.Count - (i-1)].Orderdetails.Count
                });
            }

        }

        return dashboardvm;
    }
}
