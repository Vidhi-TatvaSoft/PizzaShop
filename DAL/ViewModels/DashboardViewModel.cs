namespace DAL.ViewModels;

public class DashboardViewModel
{
    public decimal? TotalSales{get;set;}
    public int TotalOrders{get;set;}
    public decimal? AvgOrderAmount{get;set;}
    public int WaitingListCount{get;set;}
    public int NewCustomerCount{get;set;}
    public List<DashboardItemViewModel>? DashboardItemList{get;set;}
    public string AvgWaitingTime{get;set;} = null!;
}
