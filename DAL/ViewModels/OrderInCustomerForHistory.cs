namespace DAL.ViewModels;

public class OrderInCustomerForHistory
{
     public long OrderId { get; set; }

      public DateOnly OrderDate { get; set; }

      public string OrderType{get;set;}

      public string Payment {get;set;}

      public int NoOfItems{get;set;}

      public decimal Amount{get;set;}

}
