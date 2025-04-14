using System.Linq.Expressions;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Service;

public class OrderAppTableService : IOrderAppTableService
{
    private readonly PizzashopDbContext _context;

    public OrderAppTableService(PizzashopDbContext context)
    {
        _context = context;
    }

    #region Get All sectins
    public List<SectionViewModelForOrderAppTable> GetSectionList()
    {
        List<SectionViewModelForOrderAppTable> SectionList = _context.Sections.Where(x => x.Isdelete == false).OrderBy(x => x.SectionId)
                        .Select(x => new SectionViewModelForOrderAppTable
                        {
                            SectionId = x.SectionId,
                            SectionName = x.SectionName,
                            AvailableCount = 3,
                            RunningCount = 4,
                            AssignedCount = 2
                        }).ToList();
        if (SectionList != null)
        {
            return SectionList;
        }
        return null;
    }
    #endregion

    #region GetTableDetailsBySection
    public List<TableViewModelForOrderAppTable> GetTableDetailsBySection(long SectionId)
    {
        List<TableViewModelForOrderAppTable> tableListBySection = _context.Tables.Where(x => x.Isdelete == false && x.Section.SectionId == SectionId)
                        .Select(t => new TableViewModelForOrderAppTable
                        {
                            SectionId = t.SectionId,
                            TableId = t.TableId,
                            TableName = t.TableName,
                            Capacity = t.Capacity,
                            Status = t.Status,
                            Totaltime = (DateTime)t.CreatedAt,
                            TotalSpend = 0
                        }).ToList();
        if (tableListBySection != null)
        {
            return tableListBySection;
        }
        return null;
    }
    #endregion

    #region IsCustomerPresent
    public long IsCustomerPresent(string Email)
    {
        return _context.Customers.FirstOrDefault(x => x.Email == Email && x.Isdelete == false).CustomerId;
    }
    #endregion

    #region AddCustomer
    public async Task<bool> AddCustomer(WaitingTokenDetailsViewModel waitingTokenvm, long userId)
    {
        Customer customer = new();
        customer.CustomerName = waitingTokenvm.Name;
        customer.Email = waitingTokenvm.Email;
        customer.Phoneno = waitingTokenvm.Mobileno;
        customer.CreatedBy = userId;
        await _context.AddAsync(customer);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

    #region AddCustomerToWaitingList
    public async Task<bool> AddCustomerToWaitingList(WaitingTokenDetailsViewModel waitingTokenvm, long userId)
    {
        try{
            long customerId = IsCustomerPresent(waitingTokenvm.Email);

        Waitinglist waitinglist = new();
        waitinglist.CustomerId = customerId;
        waitinglist.NoOfPerson = waitingTokenvm.NoOfPerson;
        waitinglist.SectionId = waitingTokenvm.SectionID;
        await _context.AddAsync(waitinglist);
        await _context.SaveChangesAsync();
        return true;

        }catch(Exception e){
            return false;
        }
    }

    #endregion
}
