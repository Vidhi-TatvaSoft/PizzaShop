using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppKotService
{
   Task<List<KotCardDetailsViewModel>> GetDetailsByCategory(long categoryId, string status);
}