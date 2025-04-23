using DAL.Models;

namespace DAL.ViewModels;

public class OrderAppMenuViewModel
{
    public List<Category> categoryList {get;set;}

    public List<ItemsViewModel> itemsListByCategory{get;set;}

    public List<ModifierGroupForItem> modifirsByItemList{get;set;}

    public OrderDetaIlsInvoiceViewModel orderdetails{get;set;}

}
