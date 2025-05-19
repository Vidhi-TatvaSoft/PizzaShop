namespace DAL.ViewModels;

public class ItemDetailsForKot
{
    public long ItemId { get; set; }

    public long CategoryId { get; set; }

    public long OrderDetailId { get; set; }

    public string ItemName { get; set; }

    public int PendingItem { get; set; }

    public int ReadyItem { get; set; }

    public int Quantity { get; set; }

    public string ItemInstruction { get; set; } = null!;

    public List<ModifiersforItemInKot> ModifiersInItem { get; set; }
    public string? ModiiferInItemString { get; set;  }
}
