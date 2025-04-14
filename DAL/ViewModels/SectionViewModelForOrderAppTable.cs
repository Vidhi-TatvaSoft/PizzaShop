using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class SectionViewModelForOrderAppTable
{
    public long SectionId { get; set; }
    
    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Section Name cannot exceed 50 characters.")]
    public string SectionName { get; set; } = null!;

    public int AvailableCount {get;set;}

    public int AssignedCount {get;set;}

    public int RunningCount {get;set;}
}
