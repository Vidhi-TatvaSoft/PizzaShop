using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Newtonsoft.Json;
using Npgsql;

namespace BLL.Service;

public class OrderAppKotService : IOrderAppKotService
{
    private readonly PizzashopDbContext _context;

    public OrderAppKotService(PizzashopDbContext context)
    {
        _context = context;
    }

    #region GetDetailsByCategorypaginationSP
    public async Task<PaginationViewModel<KotCardDetailsViewModel>> GetDetailsByCategorypaginationSP(long categoryId, string status, int pageNumber, int pageSize = 5)
    {
        try
        {

            NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            connection.Open();
            var result = await connection.QuerySingleAsync<string>("SELECT getKotDetails(@inputStatus)", new { inputStatus = status });

            // var json = JsonSerializer.Serialize(result);
            var paginationViewModel = JsonConvert.DeserializeObject<List<KotCardDetailsViewModel>>(result);
            
            if (categoryId == 0)
            {
                paginationViewModel = paginationViewModel.Where(x => (status == "Ready") ? x.ItemsInOneCard.Any(i => i.ReadyItem > 0) : x.ItemsInOneCard.
                Any(i => i.Quantity > 0)).ToList();
            }
            else
            {
                paginationViewModel = paginationViewModel.Where(x => x.ItemsInOneCard.Any(y => y.CategoryId == categoryId) && ((status == "Ready") ? x.ItemsInOneCard.Any(i => i.ReadyItem > 0) : x.ItemsInOneCard.Any(i => i.Quantity > 0))).ToList();
                paginationViewModel.ForEach(x => x.ItemsInOneCard = x.ItemsInOneCard.Where(y => y.CategoryId == categoryId).ToList());
            }
            int totalCount = paginationViewModel.Count();
            var items = paginationViewModel.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            connection.Close();
            return new PaginationViewModel<KotCardDetailsViewModel>(items, totalCount, pageNumber, pageSize);
        }
        catch (Exception e)
        {
            return null;
        }
    }
    #endregion

    #region GetDetailsByCategorypagination
    public async Task<PaginationViewModel<KotCardDetailsViewModel>> GetDetailsByCategorypagination(long categoryId, string status, int pageNumber, int pageSize = 5)
    {
        try
        {

            List<Kot> data = await _context.Kots.Include(x => x.Order).ThenInclude(x => x.Orderdetails).ThenInclude(x => x.Item).ThenInclude(x => x.Category)
                                        .Include(x => x.Order).ThenInclude(x => x.Orderdetails).ThenInclude(x => x.Modifierorders).ThenInclude(x => x.Modifier)
                                        .Include(x => x.Order).ThenInclude(x => x.Assigntables).ThenInclude(x => x.Table).ThenInclude(x => x.Section)
                                        .Where(x => x.Isdelete == false).ToListAsync();

            if (categoryId == 0)
            {
                // data =data.Where(x => x.Order.Any(od => !od.Item.Isdelete && ((status == "Ready") ? (od.ReadyQuantity > 0) : (od.Quantity - od.ReadyQuantity > 0))));

                var kotdetailsall = data.Where(x => x.Isdelete == false && x.Order.Orderdetails.Any(od => !od.Item.Isdelete && ((status == "Ready") ? (od.ReadyQuantity > 0) : (od.Quantity - od.ReadyQuantity > 0))))
                            .Select(x => new KotCardDetailsViewModel
                            {
                                OrderId = x.Order.OrderId,
                                orderDate = x.Order.OrderDate,
                                OrderInstruction = x.Order.OtherInstruction,
                                SectionId = x.Order.Table.SectionId,
                                SectionName = x.Order.Table.Section.SectionName,
                                tableList = x.Order.Assigntables
                                            .Select(t => new Table
                                            {
                                                TableId = t.Table.TableId,
                                                TableName = t.Table.TableName,
                                            }).ToList(),

                                ItemsInOneCard = x.Order.Orderdetails.Where(x => x.Isdelete == false)
                                            .Select(k => new ItemDetailsForKot
                                            {
                                                ItemId = k.ItemId,
                                                OrderDetailId = k.OrderdetailId,
                                                ItemName = k.Item.ItemName,
                                                ItemInstruction = k.ExtraInstruction,
                                                PendingItem = k.Quantity - (int)k.ReadyQuantity,
                                                ReadyItem = (int)k.ReadyQuantity,
                                                Quantity = status == "InProgress" ? (k.Quantity - (int)k.ReadyQuantity) : (int)k.ReadyQuantity,
                                                ModifiersInItem = k.Modifierorders
                                                    .Select(m => new ModifiersforItemInKot
                                                    {
                                                        ModifierId = m.ModifierId,
                                                        ModifierName = m.Modifier.ModifierName,
                                                    }).ToList()
                                            }).ToList()
                            }).ToList();

                int totalCount = kotdetailsall.Count();
                var items = kotdetailsall.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return new PaginationViewModel<KotCardDetailsViewModel>(items, totalCount, pageNumber, pageSize);
            }
            var kotdetails = data.Where(x => (x.Isdelete == false) && x.Order.Orderdetails.Any(i => i.Item.CategoryId == categoryId) && x.Order.Orderdetails.Any(od => !od.Item.Isdelete && ((status == "Ready") ? (od.ReadyQuantity > 0) : (od.Quantity - od.ReadyQuantity > 0))))
                            .Select(x => new KotCardDetailsViewModel
                            {
                                OrderId = x.Order.OrderId,
                                orderDate = x.Order.OrderDate,
                                OrderInstruction = x.Order.OtherInstruction,
                                SectionId = x.Order.Table.SectionId,
                                SectionName = x.Order.Table.Section.SectionName,
                                tableList = x.Order.Assigntables
                                            .Select(t => new Table
                                            {
                                                TableId = t.Table.TableId,
                                                TableName = t.Table.TableName,
                                            }).ToList(),

                                ItemsInOneCard = x.Order.Orderdetails.Where(x => x.Item.CategoryId == categoryId && x.Isdelete == false)
                                            .Select(k => new ItemDetailsForKot
                                            {
                                                ItemId = k.ItemId,
                                                OrderDetailId = k.OrderdetailId,
                                                ItemName = k.Item.ItemName,
                                                ItemInstruction = k.ExtraInstruction,
                                                PendingItem = k.Quantity - (int)k.ReadyQuantity,
                                                ReadyItem = (int)k.ReadyQuantity,
                                                Quantity = status == "InProgress" ? (k.Quantity - (int)k.ReadyQuantity) : (int)k.ReadyQuantity,
                                                ModifiersInItem = k.Modifierorders
                                                    .Select(m => new ModifiersforItemInKot
                                                    {
                                                        ModifierId = m.ModifierId,
                                                        ModifierName = m.Modifier.ModifierName,
                                                    }).ToList()
                                            }).ToList()
                            }).ToList();

            int totalCount1 = kotdetails.Count();
            var items1 = kotdetails.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PaginationViewModel<KotCardDetailsViewModel>(items1, totalCount1, pageNumber, pageSize);
        }
        catch (Exception e)
        {
            return new PaginationViewModel<KotCardDetailsViewModel>(null, 0, pageNumber, pageSize); ;
        }
    }
    #endregion

    #region GetDetailsOfCardForSelectedOrder
    public async Task<KotCardDetailsViewModel> GetDetailsOfCardForSelectedOrder(long orderid, long catid, string status, int pageNumber = 1, int pageSize = 5)
    {
        try
        {
            PaginationViewModel<KotCardDetailsViewModel> kotcardDetails = await GetDetailsByCategorypaginationSP(catid, status, pageNumber, pageSize);
            var pericularOrderDetails = kotcardDetails.Items.Where(x => x.OrderId == orderid).FirstOrDefault();
            if (pericularOrderDetails == null)
            {
                return new KotCardDetailsViewModel();
            }
            return pericularOrderDetails;
        }
        catch (Exception e)
        {
            return new KotCardDetailsViewModel();
        }
    }
    #endregion

    public async Task<bool> ChangeItemQuantitiesAndStatus(int[] orderdetailIdarr, int[] itemquantityarr, string status, long userId)
    {
        try
        {
            if (orderdetailIdarr.Length != itemquantityarr.Length) return false;


            
            // NpgsqlConnection connection = new NpgsqlConnection("Host=localhost;Database=pizzashopDb;Username=postgres;password=Tatva@123");
            // connection.Open();
            // var IsUpdated = await connection.QuerySingleAsync<bool>
            // ("SELECT ChangeItemQuantitiesAndStatus(@orderdetailIds, @itemquantity, @inputStatus, @ModifiedBy)",
            //  new { orderdetailIds = orderdetailIdarr, itemquantity = itemquantityarr, inputStatus = status, ModifiedBy = userId });
            // connection.Close();
            // return IsUpdated;



            for (int i = 0; i < orderdetailIdarr.Length; i++)
            {
                var orderDetail = _context.Orderdetails.FirstOrDefault(x => x.OrderdetailId == orderdetailIdarr[i] && x.Isdelete == false);
                if (orderDetail != null)
                {
                    if (status == "InProgress")
                    {
                        orderDetail.ReadyQuantity += itemquantityarr[i];
                        orderDetail.ModifiedAt = DateTime.Now;
                        orderDetail.ModifiedBy = userId;
                        _context.Update(orderDetail);
                    }
                    else
                    {
                        orderDetail.ReadyQuantity -= itemquantityarr[i];
                        orderDetail.ModifiedAt = DateTime.Now;
                        orderDetail.ModifiedBy = userId;
                        _context.Update(orderDetail);
                    }
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

}
