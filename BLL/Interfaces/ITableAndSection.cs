using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface ITableAndSection
{
    public List<Section> GetAllSections();
    public PaginationViewModel<TableViewModel> GetTableBySection(long? sectionID, string search = "", int pageNumber = 1, int pageSize = 5);
    Task<bool> AddSection(Section section, long userID);
    Task<bool> EditSection(Section section,long userId);
    Task<bool> DeleteSection(long sectionID);
    Task<bool> AddTable(TableViewModel table, long userId);
     Task<bool> EditTable(TableViewModel table, long userId);
     Task<Table> getTableByTableId(long id);
    Task<bool> DeleteTable(long id);
}

