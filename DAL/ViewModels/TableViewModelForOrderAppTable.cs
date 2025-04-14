namespace DAL.ViewModels;

public class TableViewModelForOrderAppTable
{

    public long SectionId { get; set; }

    public long TableId { get; set; }

    public string TableName { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } = null!;

    public decimal TotalSpend {get;set;}

    public DateTime Totaltime {get;set;}

}
