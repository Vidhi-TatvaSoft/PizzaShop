namespace DAL.ViewModels;

public class OrderDetailsViewModel
{
    //customer details
     public long CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public int? Phoneno { get; set; }

    public string? Email { get; set; }


    //table details
    public long TableId { get; set; }

    public long SectionId { get; set; }

    public string TableName { get; set; } = null!;

    public string SectionName { get; set; } = null!;

    
}
