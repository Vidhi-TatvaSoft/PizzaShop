using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppMenuService
{
    
    public List<ItemsViewModel> GetItemByCategory(long categoryId, string searchText = "");

    Task<bool> FavouriteItemManage(long itemId, bool IsFavourite);

    public List<ModifierGroupForItem> GetModifiersByItemId(long itemId);

    Task<bool> SaveCustomerDetails(long customerId, string name, long mobileNo, int NoofPersons, long userId);

    OrderDetaIlsInvoiceViewModel GetOrderDetailsByCustomerId(long customerId);

    Task<OrderDetaIlsInvoiceViewModel> UpdateOrderDetailPartialView( List<List<int>> itemList,OrderDetaIlsInvoiceViewModel orderDetailsvm);

    Task<OrderDetaIlsInvoiceViewModel> RemoveItemfromOrderDetailPartialView(List<List<int>> itemList, int count, OrderDetaIlsInvoiceViewModel orderDetails);

    Task<OrderDetaIlsInvoiceViewModel> SaveOrderDetails(List<int> orderDetailId, OrderDetaIlsInvoiceViewModel orderDetails);

    Task<long> SaveRatings(long customerId,int foodreview, int serviceReview,int ambienceReview, string reviewtext );

    Task<bool> IsAllItemReady(List<int> orderDetailId,OrderDetaIlsInvoiceViewModel orderDetailsvm);

    Task<OrderDetaIlsInvoiceViewModel> CompleteOrder(OrderDetaIlsInvoiceViewModel orderDetailsvm, long paymentmethodId);
    bool IsItemReady(long orderDetailId);
}
