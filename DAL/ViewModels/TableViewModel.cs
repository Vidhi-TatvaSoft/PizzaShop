namespace DAL.ViewModels;

public class TableViewModel
{
    public long TableId { get; set; }

    public long SectionId { get; set; }

    public string TableName { get; set; } = null!;

    public int Capacity { get; set; }

    public string Status { get; set; } = null!;
     public bool Isdelete { get; set; }
}
