using DAL.Models;

namespace DAL.ViewModels;

public class OrderAppWaitingListViewModel
{
    public List<OrderAppWLSectionViewModel> sectionList{get;set;}

    public List<OrderAppWLListDetailsViewModel> waitingList{get;set;}

    public WaitingTokenDetailsViewModel waitingTokenDetailsViewModel{get;set;}
}
