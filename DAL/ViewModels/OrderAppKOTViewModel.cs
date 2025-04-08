using DAL.Models;

namespace DAL.ViewModels;

public class OrderAppKOTViewModel
{
    public List<Category> categoryList {get;set;}

    public List<KotCardDetailsViewModel> kotCardsvm {get; set;}
}
