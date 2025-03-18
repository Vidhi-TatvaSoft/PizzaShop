using DAL.Models;

namespace DAL.ViewModels;

public class TableSectionViewModel
{
    public List<Section> sectionList{get;set;}

    public Section Section {get;set;}

    public PaginationViewModel<TableViewModel> TableList{get;set;}

    public TableViewModel table{get;set;}
}
