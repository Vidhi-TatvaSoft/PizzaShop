using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface ITaxAndFees
{
    public PaginationViewModel<Tax> GetTaxes(string search = "", int pageNumber = 1, int pageSize = 5);
    Task<Tax> GetTaxByName(TaxViewModel taxvm);
    Task<bool> AddTax(TaxViewModel taxvm, long userID);
    Task<TaxViewModel> GetTaxDetailsById(long taxID);
    Task<Tax> GetTaxByNameForEdit(TaxViewModel taxvm);
    Task<bool> EditTax(TaxViewModel taxvm,long userID);
    Task<bool> DeleteTax(long id);
}
