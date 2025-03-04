using DAL.Models;


namespace DAL.ViewModels;

public class MenuViewModel
{
    public List<Category> categories {get; set;}

    public Category category{get; set;}

    // public List<ItemsViewModel> itemList{get;set;}

    public ItemsViewModel item {get;set;}

    public PaginationViewModel<ItemsViewModel> Pagination { get; set; }
}

