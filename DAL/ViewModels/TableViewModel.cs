using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class TableViewModel
{
    public long TableId { get; set; }

    public long SectionId { get; set; }
    
    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
        [StringLength(20, ErrorMessage = "Table Name cannot exceed 20 characters.")]
    public string TableName { get; set; } = null!;

    [Range(0, 99, ErrorMessage = "Capacity should be Positive and cannot exceed 2 digit")]
    public int Capacity { get; set; }

    public string Status { get; set; } = null!;
     public bool Isdelete { get; set; }
}
