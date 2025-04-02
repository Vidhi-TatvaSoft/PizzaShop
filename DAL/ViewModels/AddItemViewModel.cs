using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModels;

public class AddItemViewModel
{
    public long ItemId { get; set; }

    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Item Name cannot exceed 50 characters.")]
    public string ItemName { get; set; }

    public long CategoryId { get; set; }

    public string? Description { get; set; }

    public long ItemTypeId { get; set; }

    public IFormFile ItemFormImage { get; set; }

    public string? ItemImage {get; set;}

    [Range(0, 999, ErrorMessage = "Rate should be Positive and cannot exceed 3 digit")]
    public decimal Rate { get; set; }
    
    [StringLength(20, ErrorMessage = "ShortCode cannot exceed 20 characters.")]
    public string ShortCode { get; set; }

    [Range(0, 99, ErrorMessage = "Quantity should be Positive and cannot exceed 2 digit")]
    public int Quantity { get; set; }

    public bool Isavailable { get; set; }

    public string Unit { get; set; } = null!;

    public bool Isdefaulttax { get; set; }

    [Range(0, 99, ErrorMessage = "TaxValue should be Positive and cannot exceed 2 digit")]
    public decimal TaxValue { get; set; }

    public List<ModifierGroupForItem> ModifierGroupList{get;set;}

}
