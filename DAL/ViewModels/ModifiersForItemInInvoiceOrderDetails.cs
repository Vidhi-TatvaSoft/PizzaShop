namespace DAL.ViewModels;

public class ModifiersForItemInInvoiceOrderDetails
{
    public long ModifierId { get; set; }
    public string ModifierName { get; set; }

    public decimal? Rate { get; set; }
    public int Quantity { get; set; }

    public decimal TotalOfModifierByQuantity { get; set; }
    
    public bool Isdelete { get; set; }
}
