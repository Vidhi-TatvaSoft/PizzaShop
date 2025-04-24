namespace DAL.ViewModels;

public class ItemForInvoiceOrderDetails
{
    public long ItemId { get; set; }

    public string ItemName { get; set; }

    public int Quantity { get; set; }

    public decimal Rate { get; set; }

    public string status{get;set;}

    public decimal TotalOfItemByQuantity { get; set; }

    public List<ModifiersForItemInInvoiceOrderDetails> ModifiersInItemInvoice { get; set; }

    
}
