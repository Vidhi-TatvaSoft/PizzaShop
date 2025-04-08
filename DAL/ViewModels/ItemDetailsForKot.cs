namespace DAL.ViewModels;

public class ItemDetailsForKot
{
    public long ItemId { get; set; }

    public string ItemName { get; set; }

    public int PendingItem { get; set; }

    public int ReadyItem {get; set;}

    public int Quantity { get; set; }

    public List<ModifiersforItemInKot> ModifiersInItem { get; set; }
}
