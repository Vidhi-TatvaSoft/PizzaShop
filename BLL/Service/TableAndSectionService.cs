using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class TableAndSectionService : ITableAndSection
{
    private readonly PizzashopDbContext _context;

    public TableAndSectionService(PizzashopDbContext context)
    {
        _context = context;
    }

    #region  Get All Section List
    public List<Section> GetAllSections(){
        List<Section> allsections = _context.Sections.Where(x=>x.Isdelete==false).OrderBy(x=>x.SectionId).ToList() ;
        return allsections ;
    }
    #endregion


    #region Table pagination
    public PaginationViewModel<TableViewModel> GetTableBySection(long? sectionID, string search = "", int pageNumber = 1, int pageSize = 5)
    {

        var query = _context.Tables.Where(x => x.SectionId == sectionID &&  x.Isdelete == false).OrderBy(x=>x.TableId)
            .Select(x => new TableViewModel
            {
                TableId = x.TableId,
                TableName = x.TableName,
                SectionId = x.SectionId,
                Capacity=x.Capacity,
                Status=x.Status == "Available"? "Available":"Occupied",
                Isdelete = x.Isdelete
            })
            .AsQueryable();

        //search 
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();
            query = query.Where(x =>
                x.TableName.ToLower().Contains(lowerSearchTerm)
            );
        }

        // Get total records count (before pagination)
        int totalCount = query.Count();

        // Apply pagination
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<TableViewModel>(items, totalCount, pageNumber, pageSize);
    }
    #endregion

    #region GetSectionByName
    public async Task<Section> GetSectionByName(Section section){
        var section1 =await _context.Sections.FirstOrDefaultAsync(x=>x.SectionName.ToLower().Trim()== section.SectionName.ToLower().Trim() && x.Isdelete==false);
        return section1;
    }
    #endregion


    #region Add section
    public async Task<bool> AddSection(Section section, long userID){
        if(section==null){
            return false;
        }
        Section section1=new();
        section1.SectionName=section.SectionName;
        section1.Description=section.Description;
        section1.CreatedBy=userID;
        await _context.AddAsync(section1);
        await _context.SaveChangesAsync();
        return true;
    } 
    #endregion

    #region GetSectionByNameForEdit
    public async Task<Section> GetSectionByNameForEdit(Section section){
        var section1 =await _context.Sections.FirstOrDefaultAsync(x=>x.SectionId!=section.SectionId &&x.SectionName.ToLower().Trim()== section.SectionName.ToLower().Trim() && x.Isdelete==false);
        return section1;
    }
    #endregion

    #region  edit section post
    public async Task<bool> EditSection(Section section,long userId){
        var existingSection =await _context.Sections.FirstOrDefaultAsync(x=>x.SectionId==section.SectionId && x.Isdelete==false);
        if(existingSection== null){
            return false;
        }
        var sameNameSectionPresen= await _context.Sections.FirstOrDefaultAsync(x=>x.SectionId != section.SectionId && x.SectionName.ToLower().Trim()== section.SectionName.ToLower().Trim() && x.Isdelete==false);
        if(sameNameSectionPresen != null){
            return false;
        }
        existingSection.SectionName=section.SectionName;
        existingSection.Description=section.Description;
        existingSection.ModifiedAt=DateTime.Now;
        existingSection.ModifiedBy=userId;
        _context.Update(existingSection);
        await  _context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region ckeckOccupiedTable
    public bool ckeckOccupiedTable(long sectionId){
        return _context.Tables.Any(x => x.Section.SectionId == sectionId && x.Isdelete == false && x.Status != "Available" );
    }
    #endregion

    #region Delete section
    public async Task<bool> DeleteSection(long sectionID){
        var tablesInSection = _context.Tables.Where(x=>x.SectionId==sectionID ).ToList();
        for(int i=0; i < tablesInSection.Count(); i++){
            tablesInSection[i].Isdelete=true;
            _context.Update(tablesInSection[i]);
            await _context.SaveChangesAsync();
        }

        var sectionToDelete =await _context.Sections.FirstOrDefaultAsync(x=>x.SectionId==sectionID);
        if(sectionToDelete == null){return false;}
        sectionToDelete.Isdelete=true;
        _context.Update(sectionToDelete);
        await _context.SaveChangesAsync();

        return true;

    }
    #endregion

    #region GetTableByNameInSameSection
    public async Task<Table> GetTableByNameInSameSection(TableViewModel table){
        var table1 =await _context.Tables.FirstOrDefaultAsync(x=>x.TableId!=table.TableId && x.TableName.ToLower().Trim()== table.TableName.ToLower().Trim() && x.SectionId==table.SectionId && x.Isdelete==false);
        return table1;
    }
    #endregion

    #region Add Table post
    public async Task<bool> AddTable(TableViewModel table, long userId){
        var tablepresent =await _context.Tables.FirstOrDefaultAsync(x=>x.TableName.ToLower().Trim()== table.TableName.ToLower().Trim() && x.Isdelete==false);
        if(tablepresent != null){
            return false;
        }
        if(table==null){
            return false;
        }
        Table table1=new();
        table1.TableName=table.TableName;
        table1.Capacity=table.Capacity;
        table1.SectionId = table.SectionId;
        table1.Status=table.Status;
        table1.CreatedBy=userId;
        await _context.AddAsync(table1);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region EditTable
    public async Task<bool> EditTable(TableViewModel table, long userId){
        var tablepresent =await _context.Tables.FirstOrDefaultAsync(x=>x.TableId== table.TableId && x.Isdelete==false);
        if(tablepresent==null){
            return false;
        }
        var tableNameExist =await  _context.Tables.FirstOrDefaultAsync(x=>x.TableId != table.TableId && x.TableName.ToLower().Trim()== table.TableName.ToLower().Trim() && x.Isdelete==false);
        if(tableNameExist != null)
        {return false;}
        tablepresent.TableName=table.TableName;
        tablepresent.Capacity=table.Capacity;
        tablepresent.Status=table.Status == "Available"? "Available":"Occupied";
        tablepresent.SectionId=table.SectionId;
        tablepresent.ModifiedBy=userId;
        tablepresent.ModifiedAt=DateTime.Now;
        _context.Tables.Update(tablepresent);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region getSectionByTableId
    public async Task<Table> getTableByTableId(long id){
        var table =await _context.Tables.FirstOrDefaultAsync(x=>x.TableId==id && x.Isdelete==false);
        return table;
    }
    #endregion

    #region delete table
    public async Task<bool> DeleteTable(long id){
        Table table = await _context.Tables.FirstOrDefaultAsync(x=>x.TableId==id && x.Isdelete==false);
        if(table==null){return false;}
        table.Isdelete=true;
        _context.Tables.Update(table);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

}
