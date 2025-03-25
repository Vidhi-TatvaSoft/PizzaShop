namespace DAL.ViewModels;

public class OrderViewModel
{
    public long OrderId { get; set; }

    public long CustomerId { get; set; }
    
    public string CustomerName { get; set;}
    
    public DateOnly OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public long? RatingId { get; set; }

    public int Rating { get; set; }

    public decimal TotalAmount { get; set; }

    public long PaymentmethodId { get; set; }

    public string PaymentmethodName { get; set; }

    

}
