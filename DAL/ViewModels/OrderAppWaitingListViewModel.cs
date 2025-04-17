using DAL.Models;

namespace DAL.ViewModels;

public class OrderAppWaitingListViewModel
{
    public List<OrderAppWLSectionViewModel> sectionList{get;set;}

    public List<WaitingTokenDetailsViewModel> waitingList{get;set;}

    public WaitingTokenDetailsViewModel waitingTokenDetailsViewModel{get;set;}

    public List<TableViewModelForOrderAppTable> tableList {get;set;}
}
