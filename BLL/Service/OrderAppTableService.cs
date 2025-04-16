using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

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
        List<SectionViewModelForOrderAppTable> SectionList = _context.Sections.Include(x => x.Tables).Where(x => x.Isdelete == false).OrderBy(x => x.SectionId)
                        .Select(x => new SectionViewModelForOrderAppTable
                        {
                            SectionId = x.SectionId,
                            SectionName = x.SectionName,
                            AvailableCount = x.Tables.Where(t => t.Status == "Available" && t.Isdelete == false).Count(),
                            RunningCount = x.Tables.Where(t => t.Status == "Running" && t.Isdelete == false).Count(),
                            AssignedCount = x.Tables.Where(t => t.Status == "Assigned" && t.Isdelete == false).Count()
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

    #region IsCustomerPresentInWaiting
    public async Task<bool> IsCustomerPresentInWaiting(string Email){
        return await _context.Waitinglists.AnyAsync(x => x.Isassign == false && x.Isdelete == false && x.Customer.Email == Email );
    }
    #endregion

    #region IsCustomerPresent
    public long IsCustomerPresent(string Email)
    {
        Customer customer = _context.Customers.FirstOrDefault(x => x.Email == Email && x.Isdelete == false);
        if (customer != null) return customer.CustomerId;
        else return 0;
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
        try
        {
            long customerId = IsCustomerPresent(waitingTokenvm.Email);
            if(waitingTokenvm.waitingId == 0){
                Waitinglist waitinglist = new();
                waitinglist.CustomerId = customerId;
                waitinglist.NoOfPerson = waitingTokenvm.NoOfPerson;
                waitinglist.SectionId = waitingTokenvm.SectionID;
                await _context.AddAsync(waitinglist);
                await _context.SaveChangesAsync();
                return true;
            }else{
                Waitinglist waitinglist =await _context.Waitinglists.FirstOrDefaultAsync(x => x.WaitingId == waitingTokenvm.waitingId && x.Isdelete == false && x.Isassign == false);
                waitinglist.CustomerId = customerId;
                waitinglist.NoOfPerson = waitingTokenvm.NoOfPerson;
                waitinglist.SectionId = waitingTokenvm.SectionID;
                waitinglist.ModifiedAt = DateTime.Now;
                waitinglist.ModifiedBy = userId;
                _context.Update(waitinglist);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion

    #region GetListOfCustomerWaiting
    public List<OrderAppTableWaitingDetails> GetListOfCustomerWaiting(long sectionId)
    {
        List<OrderAppTableWaitingDetails> customerWaiting = _context.Waitinglists.Include(x => x.Customer).Include(x => x.Section)
                    .Where(x => x.Isdelete == false && x.Isassign == false && x.SectionId == sectionId)
                    .Select(w => new OrderAppTableWaitingDetails
                    {
                        ID = w.WaitingId,
                        Name = w.Customer.CustomerName,
                        NoOfPerson = w.NoOfPerson,
                        customerDetails = new WaitingTokenDetailsViewModel
                        {
                            Email = w.Customer.Email,
                            Name = w.Customer.CustomerName,
                            Mobileno = (int)w.Customer.Phoneno,
                            NoOfPerson = w.NoOfPerson,
                            SectionID = w.SectionId,
                            SectionName = w.Section.SectionName
                        }
                    }).ToList();
        if (customerWaiting != null) return customerWaiting;
        else return null;
    }
    #endregion

    #region Assigntable
    public async Task<bool> Assigntable(string Email, int[] TableIds, long userId)
    {
        try
        {
            Waitinglist waitinglist = await _context.Waitinglists.Include(x => x.Customer).FirstOrDefaultAsync(x => x.Customer.Email == Email && x.Isdelete == false && x.Isassign == false);
            if (waitinglist == null) { return false; }
            waitinglist.Isassign = true;
            waitinglist.AssignedAt = DateTime.Now;
            waitinglist.ModifiedAt = DateTime.Now;
            waitinglist.ModifiedBy = userId;

            for (int i = 0; i < TableIds.Length; i++)
            {
                Assigntable assigntable = new();
                assigntable.CustomerId = waitinglist.CustomerId;
                assigntable.TableId = TableIds[i];
                assigntable.NoOfPerson = waitinglist.NoOfPerson;
                await _context.AddAsync(assigntable);

                Table table = await _context.Tables.FirstOrDefaultAsync(x => x.TableId == TableIds[i] && x.Isdelete == false);
                table.Status = "Assigned";
                table.ModifiedAt = DateTime.Now;
                table.ModifiedBy = userId;
                _context.Update(table);
                await _context.SaveChangesAsync();
            }

            _context.Update(waitinglist);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion
}
