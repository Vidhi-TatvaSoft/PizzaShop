using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;

namespace BLL.Service;

public class OrderAppTableService : IOrderAppTableService
{
    private readonly PizzashopDbContext _context;

    public OrderAppTableService(PizzashopDbContext context)
    {
        _context = context;
    }

    #region Get All sections
    public List<SectionViewModelForOrderAppTable> GetSectionList()
    {
        try
        {
            // NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            // connection.Open();
            // var result = connection.QuerySingle<string>("SELECT GetSectionList()");

            // List<SectionViewModelForOrderAppTable>? sectionList = JsonConvert.DeserializeObject<List<SectionViewModelForOrderAppTable>>(result);
            // connection.Close();
            // return sectionList!;

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
        catch (Exception e)
        {
            return null;
        }
    }
    #endregion

    #region GetTableDetailsBySection
    public List<TableViewModelForOrderAppTable> GetTableDetailsBySection(long SectionId)
    {
        try
        {
            // NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            // connection.Open();
            // var result = connection.QuerySingle<string>("SELECT GetTableDetailsBySection(@inputSectionId)", new{ inputSectionId = SectionId });
            // if(result == null) return null!;
            // List<TableViewModelForOrderAppTable>? tableListBySection = JsonConvert.DeserializeObject<List<TableViewModelForOrderAppTable>>(result);
            // connection.Close();


            List<TableViewModelForOrderAppTable> tableListBySection = _context.Tables.Where(x => x.Isdelete == false && x.Section.SectionId == SectionId)
                            .Select(t => new TableViewModelForOrderAppTable
                            {
                                SectionId = t.SectionId,
                                TableId = t.TableId,
                                TableName = t.TableName,
                                Capacity = t.Capacity,
                                Status = t.Status,
                                Totaltime = t.Status == "Running" || t.Status == "Assigned" ? (t.Assigntables.FirstOrDefault(x => !x.Isdelete) != null ? (DateTime)t.Assigntables.FirstOrDefault(x => !x.Isdelete).CreatedAt : DateTime.Now) : DateTime.Now,
                                TotalSpend = t.Status == "Running" ? (t.Assigntables.FirstOrDefault(x => !x.Isdelete) != null ? (t.Assigntables.FirstOrDefault(x => !x.Isdelete).Order != null ? t.Assigntables.FirstOrDefault(x => !x.Isdelete).Order.TotalAmount : 0) : 0) : 0
                            }).ToList();
            if (tableListBySection.Count == 0)
            {
                return null!;
            }
            else
            {
                return tableListBySection;
            }
        }
        catch (Exception e)
        {
            return null!;
        }
    }
    #endregion

    #region IsCustomerPresentInWaiting
    public async Task<bool> IsCustomerPresentInWaiting(string Email)
    {
        try
        {
            return await _context.Waitinglists.AnyAsync(x => x.Isassign == false && x.Isdelete == false && x.Customer.Email == Email);
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion

    #region  IsCustomerPresentInWaitingUpdate
    public async Task<bool> IsCustomerPresentInWaitingUpdate(string Email, long waitingId)
    {
        try
        {
            return await _context.Waitinglists.AnyAsync(x => x.Isassign == false && x.Isdelete == false && x.Customer.Email == Email && x.WaitingId != waitingId);
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion

    #region IsCustomerPresent
    public long IsCustomerPresent(string Email)
    {
        try
        {
            Customer customer = _context.Customers.FirstOrDefault(x => x.Email == Email && x.Isdelete == false);
            if (customer != null) return customer.CustomerId;
            else return 0;
        }
        catch (Exception e)
        {
            return 0;
        }
    }
    #endregion

    #region IsCustomerAlreadyAssigned
    public bool IsCustomerAlreadyAssigned(string Email)
    {
        return _context.Assigntables.Any(at => !at.Isdelete && at.Customer.Email == Email);
    }
    #endregion

    #region AddEditCustomer
    public async Task<bool> AddEditCustomer(WaitingTokenDetailsViewModel waitingTokenvm, long userId)
    {
        try
        {
           
            NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            connection.Open();
            await connection.ExecuteAsync("CALL AddEditCustomer(@inpEmail, @inpCustomerName, @inpCustomerNo, @ModifiedBy)", new { inpEmail = waitingTokenvm.Email, inpCustomerName = waitingTokenvm.Name, inpCustomerNo = waitingTokenvm.Mobileno, ModifiedBy = userId });
            connection.Close();
            return true;

            // Customer? presentcustomer = await _context.Customers.FirstOrDefaultAsync(x => x.Email == waitingTokenvm.Email && x.Isdelete == false);
            // if (presentcustomer != null)
            // {
            //     presentcustomer.CustomerName = waitingTokenvm.Name;
            //     presentcustomer.Email = waitingTokenvm.Email;
            //     presentcustomer.Phoneno = waitingTokenvm.Mobileno;
            //     presentcustomer.ModifiedAt = DateTime.Now;
            //     presentcustomer.ModifiedBy = userId;
            //     _context.Update(presentcustomer);
            // }
            // else
            // {
            //     Customer customer = new();
            //     customer.CustomerName = waitingTokenvm.Name;
            //     customer.Email = waitingTokenvm.Email;
            //     customer.Phoneno = waitingTokenvm.Mobileno;
            //     customer.CreatedBy = userId;
            //     await _context.AddAsync(customer);
            // }

            // await _context.SaveChangesAsync();
            // return true;
            
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion

    #region AddCustomerToWaitingList
    public async Task<bool> AddEditCustomerToWaitingList(WaitingTokenDetailsViewModel waitingTokenvm, long userId)
    {
        try
        {
            // using NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            // connection.Open();
            // await connection.ExecuteAsync("CALL AddEditCustomerToWaitingList(@inpEmail, @inpWaitingId, @inpNoOfPerson, @inpSectionId, @ModifiedBy)",
            //                                 new {
            //                                     inpEmail = waitingTokenvm.Email,
            //                                     inpWaitingId = waitingTokenvm.waitingId,
            //                                     inpNoOfPerson = waitingTokenvm.NoOfPerson,
            //                                     inpSectionId = waitingTokenvm.SectionID,
            //                                     ModifiedBy = userId });
            // connection.Close();
            // return true;
        

            long customerId = IsCustomerPresent(waitingTokenvm.Email);
            if (waitingTokenvm.waitingId == 0)
            {
                Waitinglist waitinglist = new();
                waitinglist.CustomerId = customerId;
                waitinglist.NoOfPerson = waitingTokenvm.NoOfPerson;
                waitinglist.SectionId = waitingTokenvm.SectionID;
                waitinglist.CreatedBy = userId;
                await _context.AddAsync(waitinglist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                Waitinglist? waitinglist = await _context.Waitinglists.FirstOrDefaultAsync(x => x.WaitingId == waitingTokenvm.waitingId && x.Isdelete == false && x.Isassign == false);
                waitinglist!.CustomerId = customerId;
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
        try
        {
            // using NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            // connection.Open();
            // var result =  connection.QuerySingle<string>("SELECT GetListOfCustomerWaiting(@inpSectionId)",new { inpSectionId = sectionId });
            // if (result == null) return new List<OrderAppTableWaitingDetails>();
            // List<OrderAppTableWaitingDetails>? customerWaiting = JsonConvert.DeserializeObject<List<OrderAppTableWaitingDetails>>(result);
            // if (customerWaiting == null) return null!;
            // connection.Close();
            // return customerWaiting;
            


            List<OrderAppTableWaitingDetails> customerWaiting = _context.Waitinglists.Include(x => x.Customer).Include(x => x.Section)
                        .Where(x => x.Isdelete == false && x.Isassign == false && x.SectionId == sectionId)
                        .Select(w => new OrderAppTableWaitingDetails
                        {
                            ID = w.WaitingId,
                            Name = w.Customer.CustomerName,
                            NoOfPerson = w.NoOfPerson,
                            customerDetails = new WaitingTokenDetailsViewModel
                            {
                                Email = w.Customer.Email!,
                                Name = w.Customer.CustomerName,
                                Mobileno = (long)w.Customer.Phoneno!,
                                NoOfPerson = w.NoOfPerson,
                                SectionID = w.SectionId,
                                SectionName = w.Section.SectionName
                            }
                        }).ToList();
            if (customerWaiting != null) return customerWaiting;
            else return null!;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    #endregion

    #region Assigntable
    public async Task<bool> Assigntable(string Email, int[] TableIds, long userId)
    {
        try
        {
            //     long waitingId = _context.Waitinglists.FirstOrDefault(w => w.Customer.Email == Email && w.Isdelete == false && w.Isassign == false)?.WaitingId ?? 0;
            //     long sectionId = _context.Waitinglists.FirstOrDefault(w => w.Customer.Email == Email && w.Isdelete == false && w.Isassign == false)?.SectionId ?? 0;
            //     if (waitingId == 0 || sectionId == 0 || TableIds.Length == 0) { return false; }

            //    using NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            //     connection.Open();
            //     await connection.ExecuteAsync("CALL AssignTable(@inputTableIds, @inputWaitingId, @inputSectionId, @ModifiedBy)", new { inputTableIds = TableIds,inputWaitingId = waitingId, inputSectionId = sectionId,  ModifiedBy = userId });
            //      connection.Close();
            //     return true;
        
        

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
                assigntable.CreatedBy = userId;
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

