using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class ItemsViewModel
{
    public long ItemId { get; set; }

    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Item Name cannot exceed 50 characters.")]
    public string ItemName { get; set; } = null!;

    public long CategoryId { get; set; }

    public long ItemTypeId { get; set; }

    public string TypeImage { get; set; } = null!;

    [Range(0, 999, ErrorMessage = "Rate should be Positive and cannot exceed 3 digit")]
    public decimal Rate { get; set; }

    [Range(0, 99, ErrorMessage = "Quantity should be Positive and cannot exceed 2 digit")]
    public int Quantity { get; set; }

    public string? ItemImage { get; set; }

    public bool? Isavailable { get; set; }

    public bool Isdelete { get; set; }
    

}
