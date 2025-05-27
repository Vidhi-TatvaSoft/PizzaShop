using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;
using NuGet.Protocol.Plugins;

namespace BLL.Service;

public class OrderAppWaitingService : IOrderAppWaitingService
{
    private readonly PizzashopDbContext _context;

    public OrderAppWaitingService(PizzashopDbContext context)
    {
        _context = context;
    }

    #region getAllsection
    public async Task<List<OrderAppWLSectionViewModel>> GetAllSection()
    {
        NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
        connection.Open();
        var result = await connection.QuerySingleAsync<string>("SELECT GetAllSection()");
        List<OrderAppWLSectionViewModel>? OrderAppWLSectionViewModel = JsonConvert.DeserializeObject<List<OrderAppWLSectionViewModel>>(result);
        connection.Close();
        return OrderAppWLSectionViewModel!;


        // return _context.Sections.Where(x => x.Isdelete == false).OrderBy(x => x.SectionId)
        //         .Select(x => new OrderAppWLSectionViewModel
        //         {
        //             SectionId = x.SectionId,
        //             SectionName = x.SectionName,
        //             WaitingCount = _context.Waitinglists.Count(w => w.SectionId == x.SectionId && w.Isassign == false && w.Isdelete == false)
        //         }).ToList();
    }
    #endregion


    #region GetWaitingListBySection
    public List<WaitingTokenDetailsViewModel> GetWaitingListBySection(long sectionId)
    {
        try
        {

            var data = _context.Waitinglists.Include(x => x.Customer).Include(x => x.Section)
                        .Where(x => x.Isassign == false && x.Isdelete == false)
                        .OrderBy(x => x.WaitingId).ToList();

            if (sectionId == 0)
            {
                List<WaitingTokenDetailsViewModel> waitinglist = data.
                                    Select(w => new WaitingTokenDetailsViewModel
                                    {
                                        waitingId = w.WaitingId,
                                        CreatedAt = (DateTime)w.CreatedAt,
                                        Name = w.Customer.CustomerName,
                                        NoOfPerson = w.NoOfPerson,
                                        Mobileno = (long)w.Customer.Phoneno,
                                        Email = w.Customer.Email,
                                        SectionID = w.SectionId,
                                        SectionName = w.Section.SectionName
                                    }).ToList();
                return waitinglist;
            }
            else
            {
                List<WaitingTokenDetailsViewModel> waitinglist = data.Where(x => x.SectionId == sectionId)
                                    .Select(w => new WaitingTokenDetailsViewModel
                                    {
                                        waitingId = w.WaitingId,
                                        CreatedAt = (DateTime)w.CreatedAt,
                                        Name = w.Customer.CustomerName,
                                        NoOfPerson = w.NoOfPerson,
                                        Mobileno = (long)w.Customer.Phoneno,
                                        Email = w.Customer.Email,
                                        SectionID = w.SectionId,
                                        SectionName = w.Section.SectionName
                                    }).ToList();
                return waitinglist;
            }
        }
        catch (Exception e)
        {
            return new List<WaitingTokenDetailsViewModel>();
        }
    }
    #endregion

    #region GetWaitingListBySectionSP
    public async Task<List<WaitingTokenDetailsViewModel>> GetWaitingListBySectionSP(long sectionId)
    {
        try
        {
            NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            connection.Open();
            var result = await connection.QuerySingleAsync<string>("SELECT GetWaitingListBySection()");
            connection.Close();
            if (string.IsNullOrEmpty(result))
            {
                return new List<WaitingTokenDetailsViewModel>();
            }
            List<WaitingTokenDetailsViewModel>? waitingList = JsonConvert.DeserializeObject<List<WaitingTokenDetailsViewModel>>(result);
            if (sectionId == 0)
            {
                waitingList = waitingList!.ToList();
            }
            else
            {
                waitingList = waitingList!.Where(x => x.SectionID == sectionId).ToList();
            }
            return waitingList!;

        }
        catch (Exception e)
        {
            return new List<WaitingTokenDetailsViewModel>();
        }
    }
    #endregion


    #region GetWaitingTokenDetailsById
    public WaitingTokenDetailsViewModel GetWaitingTokenDetailsById(long waitingId)
    {
        try
        {

            WaitingTokenDetailsViewModel? tokendetails = _context.Waitinglists.Include(x => x.Customer).Include(x => x.Section)
                                                        .Where(x => x.WaitingId == waitingId && x.Isassign == false && x.Isdelete == false)
                                                        .Select(w => new WaitingTokenDetailsViewModel
                                                        {
                                                            waitingId = waitingId,
                                                            Email = w.Customer.Email,
                                                            Name = w.Customer.CustomerName,
                                                            Mobileno = (long)w.Customer.Phoneno,
                                                            NoOfPerson = w.NoOfPerson,
                                                            SectionID = w.SectionId,
                                                            SectionName = w.Section.SectionName
                                                        }).ToList().FirstOrDefault();
            return tokendetails == null ? null : tokendetails;

        }
        catch (Exception e)
        {
            return new WaitingTokenDetailsViewModel();
        }
    }
    #endregion

    #region GetWaitingTokenDetailsByIdSP
    public async Task<WaitingTokenDetailsViewModel> GetWaitingTokenDetailsByIdSP(long waitingId)
    {
        try
        {
            NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            connection.Open();
            var result = await connection.QuerySingleAsync<string>("SELECT GetWaitingTokenDetailsById(@inputWaitingId)", new { inputWaitingId = waitingId });
            if (string.IsNullOrEmpty(result))
            {
                return new WaitingTokenDetailsViewModel();
            }
            connection.Close();
            WaitingTokenDetailsViewModel? tokendetails = JsonConvert.DeserializeObject<WaitingTokenDetailsViewModel>(result);
            return tokendetails!;
        }
        catch (Exception e)
        {
            return new WaitingTokenDetailsViewModel();
        }
    }
    #endregion


    #region DeleteWaitingToken
    public async Task<bool> DeleteWaitingToken(long waitingId, long userId)
    {
        try
        {
            Waitinglist? waitingList = await _context.Waitinglists.FirstOrDefaultAsync(x => x.WaitingId == waitingId && x.Isassign == false && x.Isdelete == false);
            if (waitingList != null)
            {
                waitingList.Isdelete = true;
                waitingList.ModifiedAt = DateTime.Now;
                waitingList.ModifiedBy = userId;
                _context.Update(waitingList);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion

    #region DeleteWaitingTokenSP
    public async Task<bool> DeleteWaitingTokenSP(long waitingId, long userId)
    {
        try
        {
            if (waitingId == 0)
            {
                return false;
            }
            // using var connection = _context.Database.GetDbConnection();
            // var result = await connection.QuerySingleAsync<bool>("SELECT DeleteWaitingTokenSP(@inputWaitingId, @ModifiedBy)", new { inputWaitingId = waitingId, ModifiedBy = userId });
            // return result!;

            NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            connection.Open();
            await connection.ExecuteAsync("CALL DeleteWaitingTokenSP(@inputWaitingId, @ModifiedBy)", new { inputWaitingId = waitingId, ModifiedBy = userId });
            connection.Close();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion


    #region GetTableBySection
    public List<TableViewModel> GetTableBySection(long sectionID)
    {
        try
        {
            return _context.Tables.Where(x => x.SectionId == sectionID && x.Isdelete == false && x.Status == "Available")
                    .Select(t => new TableViewModel
                    {
                        TableId = t.TableId,
                        TableName = t.TableName,
                        SectionId = t.SectionId,
                        Capacity = t.Capacity,
                    }).ToList();
        }
        catch (Exception e)
        {
            return new List<TableViewModel>();
        }
    }
    #endregion

    #region GetTableBySectionSP
    public List<TableViewModel> GetTableBySectionSP(long sectionID)
    {
        try
        {
            NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            connection.Open();
            var result = connection.QuerySingle<string>("SELECT GetTableBySection(@InputSectionID)", new { InputSectionID = sectionID });
            if (string.IsNullOrEmpty(result))
            {
                return new List<TableViewModel>();
            }
            List<TableViewModel>? tablelist = JsonConvert.DeserializeObject<List<TableViewModel>>(result);
            connection.Close();
            return tablelist!;
        }
        catch (Exception e)
        {
            return new List<TableViewModel>();
        }
    }
    #endregion


    #region Get CustmerId By waitingID
    public long GetCustmerIdByWaitingId(long waitingId)
    {
        try
        {
            return _context.Waitinglists.FirstOrDefault(x => x.WaitingId == waitingId && !x.Isassign && !x.Isdelete).CustomerId;
        }
        catch (Exception e)
        {
            return 0;
        }
    }
    #endregion


    #region AssignTable
    public async Task<bool> AssignTable(int[] TableIds, long waitingId, long sectionId, long userId)
    {
        try
        {
            NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            connection.Open();
            await connection.ExecuteAsync("CALL AssignTable(@inputTableIds, @inputWaitingId, @inputSectionId, @ModifiedBy)",
            new
            {
                inputTableIds = TableIds,
                inputWaitingId = waitingId,
                inputSectionId = sectionId,
                ModifiedBy = userId
            });
            connection.Close();
            return true;


            // Waitinglist? waitinglist = await _context.Waitinglists.Include(x => x.Customer).FirstOrDefaultAsync(x => x.WaitingId == waitingId && x.Isdelete == false && x.Isassign == false);
            // if (waitinglist == null) { return false; }
            // waitinglist.Isassign = true;
            // waitinglist.SectionId = sectionId;
            // waitinglist.AssignedAt = DateTime.Now;
            // waitinglist.ModifiedAt = DateTime.Now;
            // waitinglist.ModifiedBy = userId;

            // for (int i = 0; i < TableIds.Length; i++)
            // {
            //     Assigntable assigntable = new();
            //     assigntable.CustomerId = waitinglist.CustomerId;
            //     assigntable.TableId = TableIds[i];
            //     assigntable.NoOfPerson = waitinglist.NoOfPerson;
            //     assigntable.CreatedBy = userId;
            //     await _context.AddAsync(assigntable);

            //     Table? table = await _context.Tables.FirstOrDefaultAsync(x => x.TableId == TableIds[i] && x.Isdelete == false);
            //     table.Status = "Assigned";
            //     table.ModifiedAt = DateTime.Now;
            //     table.ModifiedBy = userId;
            //     _context.Update(table);
            //     await _context.SaveChangesAsync();
            // }

            // _context.Update(waitinglist);
            // await _context.SaveChangesAsync();

            // return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion

}
