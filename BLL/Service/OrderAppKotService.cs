

using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class OrderAppKotService : IOrderAppKotService
{
    private readonly PizzashopDbContext _context;

    public OrderAppKotService(PizzashopDbContext context)
    {
        _context = context;
    }

    public async Task<List<KotCardDetailsViewModel>> GetDetailsByCategory(long categoryId, string status)
    {
        var data = await _context.Kots.Include(x => x.Order).ThenInclude(x => x.Orderdetails).ThenInclude(x => x.Item).ThenInclude(x => x.Category)
                                    .Include(x => x.Order).ThenInclude(x => x.Orderdetails).ThenInclude(x => x.Modifierorders).ThenInclude(x => x.Modifier)
                                    .Include(x => x.Order).ThenInclude(x => x.Assigntables).ThenInclude(x => x.Table).ThenInclude(x => x.Section)
                                    .Where(x => x.Isdelete  == false ).ToListAsync();
                                    
        if (categoryId == 0)
        {
            var kotdetailsall = data.Where(x => x.Isdelete == false)
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
            return kotdetailsall;
        }
        var kotdetails = data.Where(x => x.Isdelete == false)
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

                            ItemsInOneCard = x.Order.Orderdetails.Where(x =>  x.Item.CategoryId == categoryId && x.Isdelete == false)
                                        .Select(k => new ItemDetailsForKot
                                        {
                                            ItemId = k.ItemId,
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
            return kotdetails;
    }

    public async Task<KotCardDetailsViewModel> GetDetailsOfCardForSelectedOrder(long orderid,long catid,string status){
        List<KotCardDetailsViewModel> kotcardDetails =await GetDetailsByCategory(catid,status);
        var pericularOrderDetails = kotcardDetails.Where(x => x.OrderId == orderid).FirstOrDefault();
        if(pericularOrderDetails == null){
            return new KotCardDetailsViewModel();
        }
        return pericularOrderDetails;

    }

}
