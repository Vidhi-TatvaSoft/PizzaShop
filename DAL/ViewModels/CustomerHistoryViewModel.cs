namespace DAL.ViewModels;

public class CustomerHistoryViewModel
{
    public long CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public int? Phoneno { get; set; }

    public decimal MaxOrder {get;set;}

    public decimal AvgBill {get;set;}

     public DateTime ComingSince{get;set;}

    public int Visits {get;set;}

    public List<OrderInCustomerForHistory> ordersList {get;set;}






    
}
