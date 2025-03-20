using DAL.Models;

namespace DAL.ViewModels;

public class TaxANdFeesViewModel
{
    public PaginationViewModel<Tax> TaxList{get;set;}

    public TaxViewModel taxViewModel{get;set;}
}
