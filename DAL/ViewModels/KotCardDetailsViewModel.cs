using DAL.Models;

namespace DAL.ViewModels;

public class KotCardDetailsViewModel
{
    public long OrderId { get; set; }

    public DateTime orderDate {get;set;}

     public List<Table> tableList{get;set;}
    public long SectionId { get; set; }
    public string SectionName { get; set; } = null!;

    public List<ItemDetailsForKot> ItemsInOneCard { get; set; }
}
