using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
            List<ModifierGroupForItem> modifierGrpList = itemdata.Itemmodifiergroupmappings.Where(mg => mg.Isdelete == false)
                            .Select(mg => new ModifierGroupForItem
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
        var orderId = _context.Assigntables.FirstOrDefault(x => x.CustomerId == customerId && x.Isdelete == false)?.OrderId ?? 0;
        var AssignTable = data[0].Assigntables.Where(x => !x.Isdelete).ToList();
        // try
        // {
        OrderDetaIlsInvoiceViewModel orderDetailsvm = data
          .Select(o => new OrderDetaIlsInvoiceViewModel
          {
              //   OrderId = ((long)orderId == null) ? 0 : (long)orderId,
              OrderId = orderId,
              //table details
              SectionId = AssignTable[0].Table.SectionId,
              SectionName = AssignTable[0].Table.Section.SectionName,
              tableList = AssignTable.Select(x => new DAL.Models.Table
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
                            status = "In Progress",
                            Quantity = i.Quantity,
                            SpecialInstruction = i.ExtraInstruction == null ? "" : i.ExtraInstruction,
                            TotalOfItemByQuantity = Math.Round(i.Quantity * i.Item.Rate, 0),
                            OrderDetailId = i.OrderdetailId,
                            ModifiersInItemInvoice = _context.Modifierorders.Include(m => m.Modifier).Include(m => m.Orderdetail).ThenInclude(m => m.Item)
                                .Where(m => m.Orderdetail.ItemId == i.ItemId)
                                .Select(m => new ModifiersForItemInInvoiceOrderDetails
                                {
                                    ModifierId = m.ModifierId,
                                    ModifierName = m.Modifier.ModifierName,
                                    Rate = m.Modifier.Rate,
                                    Quantity = i.Quantity,
                                    TotalOfModifierByQuantity = Math.Round(i.Quantity * (decimal)m.Modifier.Rate, 0),
                                }).OrderBy(x => x.ModifierId).ToList()

                        }).ToList();
            orderDetailsvm.SubTotalAmountOfOrder = Math.Round((decimal)orderDetailsvm.ItemsInOrderDetails
                                                    .Sum(x => x.TotalOfItemByQuantity + x.ModifiersInItemInvoice.Sum(x => x.TotalOfModifierByQuantity)), 0);
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
                            TaxValue = Math.Round(tax.Tax.TaxValue / 100 * orderDetailsvm.SubTotalAmountOfOrder, 0)
                        }
                    );
                }
            }
            orderDetailsvm.TotalAmountOfOrderMain = orderDetailsvm.SubTotalAmountOfOrder + orderDetailsvm.TaxesInOrderDetails.Sum(x => x.TaxValue);
            return orderDetailsvm;
        }
        return orderDetailsvm;
        // }
        // catch (Exception e)
        // {
        //     return new OrderDetaIlsInvoiceViewModel();
        // }



    }
    #endregion

    #region UpdateOrderDetailPartialView
    public async Task<OrderDetaIlsInvoiceViewModel> UpdateOrderDetailPartialView(List<List<int>> itemList, OrderDetaIlsInvoiceViewModel orderDetailsvm)
    {
        List<ItemForInvoiceOrderDetails> itemForInvoiceOrderDetails = new();
        itemForInvoiceOrderDetails = orderDetailsvm.ItemsInOrderDetails;
        OrderDetaIlsInvoiceViewModel orderdetails = orderDetailsvm;

        // if(orderdetails.ItemsInOrderDetails == null){
        orderdetails.ItemsInOrderDetails = new();
        // }
        for (int k = 0; k < itemList.Count; k++)
        {
            long itemId = itemList[k][0];
            Console.WriteLine(k);
            // Console.WriteLine(orderDetailsvm.ItemsInOrderDetails[k].SpecialInstruction);
            ItemForInvoiceOrderDetails? itemdata = _context.Items.Where(x => x.ItemId == itemId && x.Isdelete == false)
                                                    .Select(i => new ItemForInvoiceOrderDetails
                                                    {
                                                        ItemId = i.ItemId,
                                                        ItemName = i.ItemName,
                                                        Rate = i.Rate,
                                                        status = k >= _context.Orderdetails.Where(x => x.OrderId == orderdetails.OrderId && x.Isdelete == false).Count() ? "Pending" : "In Progress",
                                                        Quantity = itemList[k][1] >= 1 ? itemList[k][1] : 1,
                                                        SpecialInstruction = itemForInvoiceOrderDetails != null ? (k >= itemForInvoiceOrderDetails.Count() ? null : itemForInvoiceOrderDetails[k].SpecialInstruction) : null,
                                                        TotalOfItemByQuantity = Math.Round(i.Rate * (itemList[k][1] >= 1 ? itemList[k][1] : 1), 0)
                                                    }).First();
            itemdata.ModifiersInItemInvoice = new();
            for (int j = 2; j < itemList[k].Count; j++)
            {
                Modifier modifier = await _context.Modifiers.FirstOrDefaultAsync(x => x.ModifierId == itemList[k][j] && x.Isdelete == false);
                ModifiersForItemInInvoiceOrderDetails mod = new();
                mod.ModifierId = modifier.ModifierId;
                mod.ModifierName = modifier.ModifierName;
                mod.Rate = modifier.Rate;
                mod.TotalOfModifierByQuantity = Math.Round((decimal)modifier.Rate * itemdata.Quantity, 0);
                itemdata.ModifiersInItemInvoice.Add(mod);
            }
            orderdetails.ItemsInOrderDetails.Add(itemdata);

        }
        orderdetails.SubTotalAmountOfOrder = Math.Round((decimal)orderdetails.ItemsInOrderDetails
                                                   .Sum(x => x.TotalOfItemByQuantity + x.ModifiersInItemInvoice.Sum(x => x.TotalOfModifierByQuantity)), 0);
        var taxedetails = _context.Taxes
        .Where(x => x.Isdelete == false).ToList();

        orderdetails.TaxesInOrderDetails = new List<TaxForOrderDetailsInvoice>();
        foreach (var tax in taxedetails)
        {
            if (tax.TaxType == "Fix Amount")
            {
                orderdetails.TaxesInOrderDetails.Add(
                    new TaxForOrderDetailsInvoice
                    {
                        TaxId = tax.TaxId,
                        TaxName = tax.TaxName,
                        TaxType = tax.TaxType,
                        TaxValue = tax.TaxValue
                    }
                );
            }
            else
            {
                orderdetails.TaxesInOrderDetails.Add(
                    new TaxForOrderDetailsInvoice
                    {
                        TaxId = tax.TaxId,
                        TaxName = tax.TaxName,
                        TaxType = tax.TaxType,
                        TaxValue = Math.Round(tax.TaxValue / 100 * orderdetails.SubTotalAmountOfOrder, 0)
                    }
                );
            }
        }
        orderdetails.TotalAmountOfOrderMain = orderdetails.SubTotalAmountOfOrder + orderdetails.TaxesInOrderDetails.Sum(x => x.TaxValue);
        return orderdetails;
    }
    #endregion

    #region RemoveItemfromOrderDetailPartialView
    public async Task<OrderDetaIlsInvoiceViewModel> RemoveItemfromOrderDetailPartialView(List<List<int>> itemList, int count, OrderDetaIlsInvoiceViewModel orderDetails)
    {
        OrderDetaIlsInvoiceViewModel orderdetails = orderDetails;
        ItemForInvoiceOrderDetails item = orderDetails.ItemsInOrderDetails[count];
        orderDetails.ItemsInOrderDetails.Remove(item);
        orderdetails.SubTotalAmountOfOrder = Math.Round((decimal)orderdetails.ItemsInOrderDetails
                                                   .Sum(x => x.TotalOfItemByQuantity + x.ModifiersInItemInvoice.Sum(x => x.TotalOfModifierByQuantity)), 2);
        if (orderDetails.SubTotalAmountOfOrder == 0)
        {
            orderDetails.ItemsInOrderDetails = null;
            orderDetails.TaxesInOrderDetails = null;
            orderDetails.TotalAmountOfOrderMain = 0;
            return orderDetails;
        }
        var taxedetails = _context.Taxes
        .Where(x => x.Isdelete == false).ToList();

        orderdetails.TaxesInOrderDetails = new List<TaxForOrderDetailsInvoice>();
        foreach (var tax in taxedetails)
        {
            if (tax.TaxType == "Fix Amount")
            {
                orderdetails.TaxesInOrderDetails.Add(
                    new TaxForOrderDetailsInvoice
                    {
                        TaxId = tax.TaxId,
                        TaxName = tax.TaxName,
                        TaxType = tax.TaxType,
                        TaxValue = tax.TaxValue
                    }
                );
            }
            else
            {
                orderdetails.TaxesInOrderDetails.Add(
                    new TaxForOrderDetailsInvoice
                    {
                        TaxId = tax.TaxId,
                        TaxName = tax.TaxName,
                        TaxType = tax.TaxType,
                        TaxValue = Math.Round(tax.TaxValue / 100 * orderdetails.SubTotalAmountOfOrder, 2)
                    }
                );
            }
        }
        orderdetails.TotalAmountOfOrderMain = orderdetails.SubTotalAmountOfOrder + orderdetails.TaxesInOrderDetails.Sum(x => x.TaxValue);
        return orderdetails;
    }
    #endregion

    #region SaveOrderDetails
    public async Task<OrderDetaIlsInvoiceViewModel> SaveOrderDetails(List<int> orderDetailId, OrderDetaIlsInvoiceViewModel orderDetailsvm)
    {
        try{

       //update order
        if (orderDetailsvm.OrderId == 0)
        {
            Order order = new();
            order.CustomerId = orderDetailsvm.CustomerId;
            order.Status = "Pending";
            order.TotalAmount = orderDetailsvm.TotalAmountOfOrderMain;
            order.PaymentmethodId  = 1;
            order.PaymentstatusId = 1;
            order.SectionId = orderDetailsvm.SectionId;
            order.TableId = orderDetailsvm.tableList[0].TableId;
            order.OtherInstruction = orderDetailsvm.OtherInstruction;
            await _context.AddAsync(order);
            await _context.SaveChangesAsync();

            orderDetailsvm.OrderId = order.OrderId;
        }
        else
        {
            Order? order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderDetailsvm.OrderId && !x.Isdelete);
            order.TotalAmount = orderDetailsvm.TotalAmountOfOrderMain;
            order.OtherInstruction = orderDetailsvm.OtherInstruction;
            _context.Update(order);
            // await _context.SaveChangesAsync();
        }

        //update orderdetails
        for(int i =0; i<orderDetailId.Count;i++){
            Orderdetail? orderdetail =await _context.Orderdetails.FirstOrDefaultAsync(od => od.OrderdetailId == orderDetailId[i] && !od.Isdelete);
            if(orderdetail != null){
                orderdetail.Quantity = orderDetailsvm.ItemsInOrderDetails[i].Quantity;
                orderdetail.ExtraInstruction = orderDetailsvm.ItemsInOrderDetails[i].SpecialInstruction;
                _context.Update(orderdetail);
                // await _context.SaveChangesAsync();
                
                List<Modifierorder> modorder = _context.Modifierorders.Where(mo => mo.OrderdetailId == orderdetail.OrderdetailId && !mo.Isdelete).ToList();
                modorder.ForEach((mo) =>{
                    mo.ModifierQuantity = orderDetailsvm.ItemsInOrderDetails[i].Quantity;
                    _context.Update(mo);
                });
                // await _context.SaveChangesAsync();
            }
        }
        for(int i=orderDetailId.Count; i<orderDetailsvm.ItemsInOrderDetails.Count; i++){
            Orderdetail orderdetail = new();
            orderdetail.OrderId = orderDetailsvm.OrderId;
            orderdetail.ItemId = orderDetailsvm.ItemsInOrderDetails[i].ItemId;
            orderdetail.Quantity = orderDetailsvm.ItemsInOrderDetails[i].Quantity;
            orderdetail.ExtraInstruction = orderDetailsvm.ItemsInOrderDetails[i].SpecialInstruction;
            orderdetail.Status = orderDetailsvm.ItemsInOrderDetails[i].status;
           await _context.AddAsync(orderdetail);
           await _context.SaveChangesAsync();
           for(int j=0; j<orderDetailsvm.ItemsInOrderDetails[i].ModifiersInItemInvoice.Count; j++){
                Modifierorder modorder = new();
                modorder.OrderdetailId = orderdetail.OrderdetailId;
                modorder.ModifierId = orderDetailsvm.ItemsInOrderDetails[i].ModifiersInItemInvoice[j].ModifierId;
                modorder.ModifierQuantity = orderDetailsvm.ItemsInOrderDetails[i].Quantity;
                await _context.AddAsync(modorder);
           }
        }
        //    await _context.SaveChangesAsync();

        //update assigntable status => assigned to running
        for(int i=0; i<orderDetailsvm.tableList.Count; i++){
            Assigntable? assigntable =await _context.Assigntables.FirstOrDefaultAsync(at => at.TableId == orderDetailsvm.tableList[i].TableId && !at.Isdelete);
            if(assigntable!=null){
                assigntable.OrderId = orderDetailsvm.OrderId;
                _context.Update(assigntable);
            }
            DAL.Models.Table? table =await _context.Tables.FirstOrDefaultAsync(t => t.TableId == orderDetailsvm.tableList[i].TableId && !t.Isdelete);
            if(table != null){
                table.Status = "Running";
                _context.Update(table);
            }
        }
        // await _context.SaveChangesAsync();


        List<Kot> kotList = _context.Kots.Where(kot => kot.OrderId == orderDetailsvm.OrderId && !kot.Isdelete).ToList();
        if(kotList.Count == 0){
            Kot kot = new();
            kot.OrderId = orderDetailsvm.OrderId;
            await _context.AddAsync(kot);
        }
        

        


        await _context.SaveChangesAsync();

        // List<ItemForInvoiceOrderDetails> saveditems = orderDetailsvm.ItemsInOrderDetails.Where(x => x.status == "In Progress").ToList();
        // for (int k = saveditems.Count(); k < itemList.Count(); k++)
        // {

        //     List<Orderdetail>? orderdetail = _context.Orderdetails.Where(od => od.ItemId == orderDetailsvm.ItemsInOrderDetails[k].ItemId && od.OrderId == orderDetailsvm.OrderId && !od.Isdelete).ToList();
        //     for (int j = 0; j < orderdetail.Count(); j++)
        //     {
        //         bool flag = true;
        //         List<Modifierorder> modorder = _context.Modifierorders.Where(mo => !mo.Isdelete && mo.OrderdetailId == orderdetail[j].OrderdetailId).OrderBy(mo => mo.ModifierId).ToList();
        //         if (modorder.Count == orderDetailsvm.ItemsInOrderDetails[k].ModifiersInItemInvoice.Count)
        //         {
        //             for (int x = 0; x < orderDetailsvm.ItemsInOrderDetails[k].ModifiersInItemInvoice.Count; x++)
        //             {
        //                 if (orderDetailsvm.ItemsInOrderDetails[k].ModifiersInItemInvoice[x].ModifierId != modorder[x].ModifierId)
        //                 {
        //                     flag = false;
        //                     break;
        //                 }
        //             }
        //         }
        //         else { flag = false; }
        //         if (flag)
        //         {
        //             orderdetail[j].Quantity++;
        //             _context.Update(orderdetail);
        //         }
        //         else
        //         {
        //         }
        //     }
        // }

        return orderDetailsvm;
         }catch(Exception e){
            return null;
         }
    }
    #endregion

}
