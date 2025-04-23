using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BLL.Service;

public class OrderAppMenuService : IOrderAppMenuService
{
    private readonly PizzashopDbContext _context;

    public OrderAppMenuService(PizzashopDbContext context)
    {
        _context = context;
    }


    #region GetItemByCategory
    public List<ItemsViewModel> GetItemByCategory(long categoryId, string searchText = "")
    {
        var allItems = _context.Items.Where(x => x.Isavailable == true && x.Isdelete == false).ToList();

        if (categoryId == -1)
        {
            allItems = allItems.Where(x => x.IsFavourite == true).ToList();
        }
        else if (categoryId == 0)
        {
            allItems = allItems;
        }
        else
        {
            allItems = allItems.Where(x => x.CategoryId == categoryId).ToList();
        }

        if (!searchText.IsNullOrEmpty())
        {
            allItems = allItems.Where(x => x.ItemName.ToLower().Trim().Contains(searchText.ToLower().Trim())).ToList();
        }

        List<ItemsViewModel> itemsList = allItems.Select(i => new ItemsViewModel
        {
            ItemId = i.ItemId,
            ItemName = i.ItemName,
            CategoryId = i.CategoryId,
            ItemTypeId = i.ItemTypeId,
            Rate = Math.Ceiling(i.Rate),
            ItemImage = i.ItemImage,
            IsFavourite = i.IsFavourite,
            Isdelete = i.Isdelete
        }).ToList();

        return itemsList;

    }
    #endregion

    #region FavouriteItemManage
    public async Task<bool> FavouriteItemManage(long itemId, bool IsFavourite)
    {
        Item? item = await _context.Items.FirstOrDefaultAsync(x => x.ItemId == itemId && x.Isdelete == false);
        if (item != null)
        {
            item.IsFavourite = IsFavourite;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region GetModifiersByItemId
    public List<ModifierGroupForItem> GetModifiersByItemId(long itemId)
    {
        Item? itemdata = _context.Items.Include(x => x.Itemmodifiergroupmappings).ThenInclude(x => x.ModifierGrp).ThenInclude(x => x.Modifiers)
                        .FirstOrDefault(x => x.ItemId == itemId && x.Isdelete == false);
        long typeId = itemdata.ItemTypeId;
        if (itemdata == null)
        {
            return new List<ModifierGroupForItem>();
        }
        else
        {
            List<ModifierGroupForItem> modifierGrpList = itemdata.Itemmodifiergroupmappings.Where(mg => mg.Isdelete == false).Select(mg => new ModifierGroupForItem
            {
                ModifierGrpId = (long)mg.ModifierGrpId,
                ModifierGrpName = mg.ModifierGrp.ModifierGrpName,
                min = mg.Minvalue,
                max = mg.Maxvalue,
                TypeId = typeId,
                modifierList = mg.ModifierGrp.Modifiers.Where(m => m.Isdelete == false).Select(m => new Modifier
                {
                    ModifierId = m.ModifierId,
                    ModifierName = m.ModifierName,
                    Rate = m.Rate
                }).ToList()
            }).ToList();
            return modifierGrpList;
        }
    }
    #endregion

    #region GetOrderDetailsByCustomerId
    public OrderDetaIlsInvoiceViewModel GetOrderDetailsByCustomerId(long customerId)
    {
        var data = _context.Customers.Include(x => x.Assigntables).ThenInclude(x => x.Table).ThenInclude(x => x.Section)
                    .Include(x => x.Assigntables).ThenInclude(x => x.Order).ThenInclude(x => x.Orderdetails)
                    .Where(x => x.CustomerId == customerId && x.Isdelete == false).ToList();

        // var orderid = data != null ? data.Select(x => x).FirstOrDefault() : 0;
        var orderId = _context.Assigntables.FirstOrDefault(x => x.CustomerId == customerId && x.Isdelete == false)?.OrderId;
        var AssignTable = data[0].Assigntables.Where(x => !x.Isdelete).ToList();
        OrderDetaIlsInvoiceViewModel orderDetailsvm = data
        .Select(o => new OrderDetaIlsInvoiceViewModel
        {
            //table details
            SectionId = AssignTable[0].Table.SectionId,
            SectionName = AssignTable[0].Table.Section.SectionName,
            tableList = AssignTable.Select(x => new Table
            {
                TableId = x.Table.TableId,
                TableName = x.Table.TableName,
                SectionId = x.Table.SectionId
            }).ToList(),

            //customerdetails
            CustomerId = o.CustomerId,
            CustomerName = o.CustomerName,
            Phoneno = o.Phoneno,
            Email = o.Email
        }).ToList()[0];

        // //orderDetails
        if (orderId != 0)
        {
            var orderDetails = _context.Orderdetails.Include(x => x.Item)
                            .Include(x => x.Modifierorders).ThenInclude(x => x.Modifier)
                            .Where(x => x.OrderId == orderId && x.Isdelete == false).ToList();

            orderDetailsvm.ItemsInOrderDetails = orderDetails
                        .Select(i => new ItemForInvoiceOrderDetails
                        {
                            ItemId = i.ItemId,
                            ItemName = i.Item.ItemName,
                            Rate = i.Item.Rate,
                            Quantity = i.Quantity,
                            TotalOfItemByQuantity = Math.Round(i.Quantity * i.Item.Rate, 2),
                            ModifiersInItemInvoice = _context.Modifierorders.Include(m => m.Modifier).Include(m => m.Orderdetail).ThenInclude(m => m.Item)
                                .Where(m => m.Orderdetail.ItemId == i.ItemId)
                                .Select(m => new ModifiersForItemInInvoiceOrderDetails
                                {
                                    ModifierId = m.ModifierId,
                                    ModifierName = m.Modifier.ModifierName,
                                    Rate = m.Modifier.Rate,
                                    Quantity = m.ModifierQuantity,
                                    TotalOfModifierByQuantity = Math.Round(m.ModifierQuantity * (decimal)m.Modifier.Rate, 2),
                                }).ToList()

                        }).ToList();
            orderDetailsvm.SubTotalAmountOfOrder = Math.Round((decimal)orderDetailsvm.ItemsInOrderDetails
                                                    .Sum(x => x.TotalOfItemByQuantity + x.ModifiersInItemInvoice.Sum(x => x.TotalOfModifierByQuantity)), 2);
            var taxedetails = _context.Taxinvoicemappings.Include(x => x.Invoice).Include(x => x.Tax)
            .Where(x => x.Invoice.OrderId == orderId).ToList();

            orderDetailsvm.TaxesInOrderDetails = new List<TaxForOrderDetailsInvoice>();
            foreach (var tax in taxedetails)
            {

                if (tax.Tax.TaxType == "Fix Amount")
                {
                    orderDetailsvm.TaxesInOrderDetails.Add(
                        new TaxForOrderDetailsInvoice
                        {
                            TaxId = tax.Tax.TaxId,
                            TaxName = tax.Tax.TaxName,
                            TaxType = tax.Tax.TaxType,
                            TaxValue = tax.Tax.TaxValue
                        }
                    );
                }
                else
                {
                    orderDetailsvm.TaxesInOrderDetails.Add(
                        new TaxForOrderDetailsInvoice
                        {
                            TaxId = tax.Tax.TaxId,
                            TaxName = tax.Tax.TaxName,
                            TaxType = tax.Tax.TaxType,
                            TaxValue = Math.Round(tax.Tax.TaxValue / 100 * orderDetailsvm.SubTotalAmountOfOrder, 2)
                        }
                    );
                }
            }
            orderDetailsvm.TotalAmountOfOrderMain = orderDetailsvm.SubTotalAmountOfOrder + orderDetailsvm.TaxesInOrderDetails.Sum(x => x.TaxValue);
            return orderDetailsvm;
        }
        return new OrderDetaIlsInvoiceViewModel();
    }
    #endregion

}
