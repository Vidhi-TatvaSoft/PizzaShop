using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppKotService
{
   // Task<List<KotCardDetailsViewModel>> GetDetailsByCategory(long categoryId, string status);
   Task<PaginationViewModel<KotCardDetailsViewModel>> GetDetailsByCategorypaginationSP(long categoryId, string status, int pageNumber, int pageSize = 5);
   Task<PaginationViewModel<KotCardDetailsViewModel>> GetDetailsByCategorypagination(long categoryId, string status,  int pageNumber = 1, int pageSize = 5);

   Task<KotCardDetailsViewModel> GetDetailsOfCardForSelectedOrder(long orderid,long catid,string status,int pageNumber = 1, int pageSize = 5);

   Task<bool> ChangeItemQuantitiesAndStatus(int[] orderdetailIdarr , int [] itemquantityarr, string status, long userId);
}