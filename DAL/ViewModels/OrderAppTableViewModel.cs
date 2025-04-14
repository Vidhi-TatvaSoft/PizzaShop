using DAL.Models;

namespace DAL.ViewModels;

public class OrderAppTableViewModel
{
    public List<SectionViewModelForOrderAppTable> sectionList {get; set;}

    public List<TableViewModelForOrderAppTable> tablesInSection {get;set;}

    public WaitingTokenDetailsViewModel waitingTokenDetailsViewModel{get;set;}
}
