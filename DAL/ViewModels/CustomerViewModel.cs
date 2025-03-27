namespace DAL.ViewModels;

public class CustomerViewModel
{
    public long CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public int? Phoneno { get; set; }

    public string? Email { get; set; }

    public DateOnly date{get;set;}

    public string TotalOrders{get;set;}
}
