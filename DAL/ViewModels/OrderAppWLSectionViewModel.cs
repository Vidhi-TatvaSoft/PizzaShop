using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class OrderAppWLSectionViewModel
{
     public long SectionId { get; set; }
    
    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Section Name cannot exceed 50 characters.")]
    public string SectionName { get; set; } = null!;

    public int WaitingCount {get;set;}
}
