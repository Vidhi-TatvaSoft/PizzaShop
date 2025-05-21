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
    public async Task<bool> FavouriteItemManage(long itemId, bool IsFavourite, long userId)
    {
        Item? item = await _context.Items.FirstOrDefaultAsync(x => x.ItemId == itemId && x.Isdelete == false);
        if (item != null)
        {
            item.IsFavourite = IsFavourite;
            item.ModifiedAt=DateTime.Now;
            item.ModifiedBy = userId;
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

    #region SaveCustomerDetails
    public async Task<bool> SaveCustomerDetails(long customerId, string name, long mobileNo, int NoofPersons, long userId)
    {
        try
        {
            Customer? customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.Isdelete);
            customer.CustomerName = name;
            customer.Phoneno = mobileNo;
            customer.ModifiedAt = DateTime.Now;
            customer.ModifiedBy = userId;
            _context.Update(customer);

            Waitinglist? waitinglist = await _context.Waitinglists.FirstOrDefaultAsync(wl => wl.CustomerId == customerId && wl.Isassign);
            waitinglist.NoOfPerson = NoofPersons;
            // waitinglist.ModifiedAt = DateTime.Now;
            waitinglist.ModifiedBy = userId;
            _context.Update(waitinglist);

            List<Assigntable> assigntables = _context.Assigntables.Where(at => at.CustomerId == customerId && !at.Isdelete).ToList();
            foreach (var table in assigntables)
            {
                table.NoOfPerson = NoofPersons;
                table.ModifiedAt = DateTime.Now;
                table.ModifiedBy = userId;
                _context.Update(table);
            }
            await _context.SaveChangesAsync();
            return true;

        }
        catch (Exception e)
        {
            return false;
        }
    }
    #endregion

    #region GetOrderDetailsByCustomerId
    public OrderDetaIlsInvoiceViewModel GetOrderDetailsByCustomerId(long customerId)
    {
        try
        {
            var data = _context.Customers.Include(x => x.Assigntables).ThenInclude(x => x.Table).ThenInclude(x => x.Section)
                        .Include(x => x.Assigntables).ThenInclude(x => x.Order).ThenInclude(x => x.Orderdetails)
                        .Where(x => x.CustomerId == customerId && x.Isdelete == false ).ToList();

            // var orderid = data != null ? data.Select(x => x).FirstOrDefault() : 0;
            var orderId = _context.Assigntables.FirstOrDefault(x => x.CustomerId == customerId && !x.Isdelete)?.OrderId ?? 0;
            var AssignTable = data[0].Assigntables.Where(x => !x.Isdelete).ToList();
            // try
            // {
            OrderDetaIlsInvoiceViewModel orderDetailsvm = data
              .Select(o => new OrderDetaIlsInvoiceViewModel
              {
                  OrderId = orderId,
                  //table details
                  SectionId = AssignTable[0].Table.SectionId,
                  SectionName = AssignTable[0].Table.Section.SectionName,
                  tableList = AssignTable.Select(x => new DAL.Models.Table
                  {
                      TableId = x.Table.TableId,
                      TableName = x.Table.TableName,
                      SectionId = x.Table.SectionId,
                      Capacity = x.Table.Capacity
                  }).ToList(),

                  //customerdetails
                  CustomerId = o.CustomerId,
                  CustomerName = o.CustomerName,
                  Phoneno = o.Phoneno,
                  Email = o.Email,
                  NumberOfPerson =AssignTable.FirstOrDefault().NoOfPerson,
                  

              }).ToList()[0];
            // //orderDetails
            if (orderId != 0)
            {
                var orderDetails = _context.Orderdetails.Include(x => x.Item)
                                .Include(x => x.Modifierorders).ThenInclude(x => x.Modifier)
                                .Where(x => x.OrderId == orderId && x.Isdelete == false).ToList();
                orderDetailsvm.OtherInstruction = _context.Orders.FirstOrDefault(o => o.OrderId == orderId && !o.Isdelete)!.OtherInstruction!;
                orderDetailsvm.OrderStatus = _context.Orders.FirstOrDefault(o => o.OrderId == orderId && !o.Isdelete)!.Status;
                orderDetailsvm.InvoiceId = _context.Invoices.FirstOrDefault(i => i.OrderId == orderId && i.CustomerId == customerId) == null ?
                                             0 : _context.Invoices.FirstOrDefault(i => i.OrderId == orderId && i.CustomerId == customerId)!.InvoiceId;
                orderDetailsvm.OrderDate = _context.Assigntables.FirstOrDefault(x => x.CustomerId == customerId && !x.Isdelete)!.Order!.OrderDate;
                orderDetailsvm.ModifiedOn =  _context.Assigntables.FirstOrDefault(x => x.CustomerId == customerId && !x.Isdelete)!.Order!.ModifiedAt == null ? 
                                                _context.Assigntables.FirstOrDefault(x => x.CustomerId == customerId && !x.Isdelete)!.Order!.OrderDate :
                                                 (DateTime)_context.Assigntables.FirstOrDefault(x => x.CustomerId == customerId && !x.Isdelete)!.Order!.ModifiedAt!;
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
                                    .Where(m => m.Orderdetail.OrderdetailId == i.OrderdetailId)
                                    .Select(m => new ModifiersForItemInInvoiceOrderDetails
                                    {
                                        ModifierId = m.ModifierId,
                                        ModifierName = m.Modifier.ModifierName,
                                        Rate = m.Modifier.Rate,
                                        Quantity = i.Quantity,
                                        TotalOfModifierByQuantity = Math.Round(i.Quantity * (decimal)m.Modifier.Rate!, 0),
                                    }).OrderBy(x => x.ModifierId).ToList()

                            }).ToList();
                orderDetailsvm.SubTotalAmountOfOrder = Math.Round((decimal)orderDetailsvm.ItemsInOrderDetails
                                                        .Sum(x => x.TotalOfItemByQuantity + x.ModifiersInItemInvoice.Sum(x => x.TotalOfModifierByQuantity)), 0);

                var taxedetails = _context.Taxes.Where(x => !x.Isdelete && (bool)x.Isenable).ToList();

                orderDetailsvm.TaxesInOrderDetails = new List<TaxForOrderDetailsInvoice>();
                foreach (var tax in taxedetails)
                {

                    if (tax.TaxType == "Fix Amount")
                    {
                        orderDetailsvm.TaxesInOrderDetails.Add(
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
                        orderDetailsvm.TaxesInOrderDetails.Add(
                            new TaxForOrderDetailsInvoice
                            {
                                TaxId = tax.TaxId,
                                TaxName = tax.TaxName,
                                TaxType = tax.TaxType,
                                TaxValue = Math.Round(tax.TaxValue / 100 * orderDetailsvm.SubTotalAmountOfOrder, 0)
                            }
                        );
                    }
                }
                orderDetailsvm.TotalAmountOfOrderMain = orderDetailsvm.SubTotalAmountOfOrder + orderDetailsvm.TaxesInOrderDetails.Sum(x => x.TaxValue);
                return orderDetailsvm;
            }
            return orderDetailsvm;
        }
        catch (Exception e)
        {
            return new OrderDetaIlsInvoiceViewModel();
        }



    }
    #endregion

    #region UpdateOrderDetailPartialView
    public async Task<OrderDetaIlsInvoiceViewModel> UpdateOrderDetailPartialView(List<List<int>> itemList, OrderDetaIlsInvoiceViewModel orderDetailsvm)
    {
        try{

        List<ItemForInvoiceOrderDetails> itemForInvoiceOrderDetails = new();
        itemForInvoiceOrderDetails = orderDetailsvm.ItemsInOrderDetails ==null? new():orderDetailsvm.ItemsInOrderDetails;
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
                                                        OrderDetailId = k >= itemForInvoiceOrderDetails.Count() ? 0 : itemForInvoiceOrderDetails[k].OrderDetailId,
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

        var taxedetails = _context.Taxes.Where(x => !x.Isdelete && (bool)x.Isenable).ToList();

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
        
        }catch(Exception e){
            return orderDetailsvm;
        }
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
        var taxedetails = _context.Taxes.Where(x => !x.Isdelete && (bool)x.Isenable).ToList();

        orderDetails.TaxesInOrderDetails = new List<TaxForOrderDetailsInvoice>();
        foreach (var tax in taxedetails)
        {

            if (tax.TaxType == "Fix Amount")
            {
                orderDetails.TaxesInOrderDetails.Add(
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
                orderDetails.TaxesInOrderDetails.Add(
                    new TaxForOrderDetailsInvoice
                    {
                        TaxId = tax.TaxId,
                        TaxName = tax.TaxName,
                        TaxType = tax.TaxType,
                        TaxValue = Math.Round(tax.TaxValue / 100 * orderDetails.SubTotalAmountOfOrder, 0)
                    }
                );
            }
        }
        orderdetails.TotalAmountOfOrderMain = orderdetails.SubTotalAmountOfOrder + orderdetails.TaxesInOrderDetails.Sum(x => x.TaxValue);
        return orderdetails;
    }
    #endregion

    #region SaveOrderDetails
    public async Task<OrderDetaIlsInvoiceViewModel> SaveOrderDetails(List<int> orderDetailId, OrderDetaIlsInvoiceViewModel orderDetailsvm, long userId)
    {
        try
        {

            //update order
            if (orderDetailsvm.OrderId == 0)
            {
                Order order = new();
                order.CustomerId = orderDetailsvm.CustomerId;
                order.Status = "Pending";
                order.TotalAmount = orderDetailsvm.TotalAmountOfOrderMain;
                order.PaymentmethodId = 1;
                order.PaymentstatusId = 1;
                order.SectionId = orderDetailsvm.SectionId;
                order.TableId = orderDetailsvm.tableList[0].TableId;
                order.OtherInstruction = orderDetailsvm.OtherInstruction;
                order.CreatedBy=userId;
                await _context.AddAsync(order);
                await _context.SaveChangesAsync();

                orderDetailsvm.OrderId = order.OrderId;
            }
            else
            {
                Order? order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderDetailsvm.OrderId && !x.Isdelete);
                order.TotalAmount = orderDetailsvm.TotalAmountOfOrderMain;
                order.OtherInstruction = orderDetailsvm.OtherInstruction;
                order.ModifiedAt=DateTime.Now;
                order.ModifiedBy=userId;
                _context.Update(order);
                await _context.SaveChangesAsync();
            }

            //update orderdetails
            for (int i = 0; i < orderDetailId.Count; i++)
            {
                Orderdetail? orderdetail = await _context.Orderdetails.FirstOrDefaultAsync(od => od.OrderdetailId == orderDetailId[i] && !od.Isdelete);
                if (orderdetail != null)
                {
                    orderdetail.Quantity = orderDetailsvm.ItemsInOrderDetails[i].Quantity;
                    orderdetail.ExtraInstruction = orderDetailsvm.ItemsInOrderDetails[i].SpecialInstruction;
                    orderdetail.ModifiedAt=DateTime.Now;
                    orderdetail.ModifiedBy=userId;
                    _context.Update(orderdetail);
                    // await _context.SaveChangesAsync();

                    List<Modifierorder> modorder = _context.Modifierorders.Where(mo => mo.OrderdetailId == orderdetail.OrderdetailId && !mo.Isdelete).ToList();
                    modorder.ForEach((mo) =>
                    {
                        mo.ModifierQuantity = orderDetailsvm.ItemsInOrderDetails[i].Quantity;
                        mo.ModifiedAt=DateTime.Now;
                        mo.ModifiedBy=userId;
                        _context.Update(mo);
                    });
                    // await _context.SaveChangesAsync();
                }
            }
            for (int i = orderDetailId.Count; i < orderDetailsvm.ItemsInOrderDetails.Count; i++)
            {
                Orderdetail orderdetail = new();
                orderdetail.OrderId = orderDetailsvm.OrderId;
                orderdetail.ItemId = orderDetailsvm.ItemsInOrderDetails[i].ItemId;
                orderdetail.Quantity = orderDetailsvm.ItemsInOrderDetails[i].Quantity;
                orderdetail.ExtraInstruction = orderDetailsvm.ItemsInOrderDetails[i].SpecialInstruction;
                orderdetail.Status = orderDetailsvm.ItemsInOrderDetails[i].status;
                orderdetail.CreatedBy=userId;
                await _context.AddAsync(orderdetail);
                await _context.SaveChangesAsync();
                orderDetailsvm.ItemsInOrderDetails[i].OrderDetailId = orderdetail.OrderdetailId;
                for (int j = 0; j < orderDetailsvm.ItemsInOrderDetails[i].ModifiersInItemInvoice.Count; j++)
                {
                    Modifierorder modorder = new();
                    modorder.OrderdetailId = orderdetail.OrderdetailId;
                    modorder.ModifierId = orderDetailsvm.ItemsInOrderDetails[i].ModifiersInItemInvoice[j].ModifierId;
                    modorder.ModifierQuantity = orderDetailsvm.ItemsInOrderDetails[i].Quantity;
                    modorder.CreatedBy=userId;
                    await _context.AddAsync(modorder);
                }
            }
            //    await _context.SaveChangesAsync();

            //update assigntable status => assigned to running
            for (int i = 0; i < orderDetailsvm.tableList.Count; i++)
            {
                Assigntable? assigntable = await _context.Assigntables.FirstOrDefaultAsync(at => at.TableId == orderDetailsvm.tableList[i].TableId && !at.Isdelete);
                if (assigntable != null)
                {
                    assigntable.OrderId = orderDetailsvm.OrderId;
                    assigntable.ModifiedAt=DateTime.Now;
                    assigntable.ModifiedBy=userId;
                    _context.Update(assigntable);
                }
                DAL.Models.Table? table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == orderDetailsvm.tableList[i].TableId && !t.Isdelete);
                if (table != null)
                {
                    table.Status = "Running";
                    table.ModifiedAt=DateTime.Now;
                    table.ModifiedBy = userId;
                    _context.Update(table);
                }
            }
            // await _context.SaveChangesAsync();


            List<Kot> kotList = _context.Kots.Where(kot => kot.OrderId == orderDetailsvm.OrderId && !kot.Isdelete).ToList();
            if (kotList.Count == 0)
            {
                Kot kot = new();
                kot.OrderId = orderDetailsvm.OrderId;
                kot.CreatedBy=userId;
                await _context.AddAsync(kot);
            }

            //fill view model
            for (int i = 0; i < orderDetailsvm.ItemsInOrderDetails.Count; i++)
            {
                orderDetailsvm.ItemsInOrderDetails[i].status = "In Progress";
            }
            var taxedetails = _context.Taxes.Where(x => !x.Isdelete && (bool)x.Isenable).ToList();

            orderDetailsvm.TaxesInOrderDetails = new List<TaxForOrderDetailsInvoice>();
            foreach (var tax in taxedetails)
            {

                if (tax.TaxType == "Fix Amount")
                {
                    orderDetailsvm.TaxesInOrderDetails.Add(
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
                    orderDetailsvm.TaxesInOrderDetails.Add(
                        new TaxForOrderDetailsInvoice
                        {
                            TaxId = tax.TaxId,
                            TaxName = tax.TaxName,
                            TaxType = tax.TaxType,
                            TaxValue = Math.Round(tax.TaxValue / 100 * orderDetailsvm.SubTotalAmountOfOrder, 0)
                        }
                    );
                }
            }

            //Add invoice
            if (orderDetailsvm.InvoiceId == 0 || orderDetailsvm.InvoiceId == null)
            {

                Invoice invoice = new();
                invoice.InvoiceNo = "#DOM" + DateTime.Now.ToString("yyyyMMddHHmmss");
                invoice.OrderId = orderDetailsvm.OrderId;
                invoice.CustomerId = orderDetailsvm.CustomerId;
                invoice.CreatedBy=userId;
                await _context.AddAsync(invoice);
                await _context.SaveChangesAsync();
                orderDetailsvm.InvoiceId = invoice.InvoiceId;
            }
            await _context.SaveChangesAsync();
            return orderDetailsvm;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    #endregion

    #region SaveRatings
    public async Task<long> SaveRatings(long customerId, int foodreview, int serviceReview, int ambienceReview, string reviewtext, long userId)
    {
        Rating? ratings = await _context.Ratings.FirstOrDefaultAsync(r => r.Food == foodreview && r.Ambience == ambienceReview && r.Service == serviceReview && r.Review == reviewtext && r.Isdelete == false);
        long ratingId;
        if (ratings == null)
        {
            Rating rating = new();
            rating.Food = foodreview;
            rating.CustomerId = customerId;
            rating.Ambience = ambienceReview;
            rating.Service = serviceReview;
            rating.Review = reviewtext;
            rating.CreatedBy = userId;
            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();
            ratingId = rating.RatingId;
            return ratingId;
        }
        return (long)ratings.RatingId;

    }
    #endregion

    #region CompleteOrder
    public async Task<OrderDetaIlsInvoiceViewModel> CompleteOrder(OrderDetaIlsInvoiceViewModel orderDetailsvm, long paymentmethodId, long userId)
    {
        try
        {
            //update order table
            Order? order = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderDetailsvm.OrderId && !x.Isdelete);
            order.TotalAmount = orderDetailsvm.TotalAmountOfOrderMain;
            order.OtherInstruction = orderDetailsvm.OtherInstruction;
            order.RatingId = orderDetailsvm.RatingId;
            order.PaymentmethodId = paymentmethodId;
            order.Status = "Completed";
            order.PaymentstatusId = 2;
            order.ModifiedAt = DateTime.Now;
            order.ModifiedBy=userId;
            _context.Update(order);
            await _context.SaveChangesAsync();

            //update orderDetail table
            for (int i = 0; i < orderDetailsvm.ItemsInOrderDetails.Count; i++)
            {
                Orderdetail? orderdetail = await _context.Orderdetails.FirstOrDefaultAsync(x => x.OrderdetailId == orderDetailsvm.ItemsInOrderDetails[i].OrderDetailId && !x.Isdelete);
                orderdetail.Status = "Completed";
                orderdetail.ModifiedAt=DateTime.Now;
                orderdetail.ModifiedBy=userId;
                _context.Update(orderdetail);

            }

            //assignatble isdelete =true
            List<Assigntable> assigntable = _context.Assigntables.Where(x => x.OrderId == orderDetailsvm.OrderId && x.CustomerId == orderDetailsvm.CustomerId && !x.Isdelete).ToList();
            for (int i = 0; i < assigntable.Count; i++)
            {
                assigntable[i].Isdelete = true;
                assigntable[i].ModifiedAt=DateTime.Now;
                assigntable[i].ModifiedBy=userId;
                _context.Update(assigntable[i]);
            }

            //table status = available
            for (int i = 0; i < orderDetailsvm.tableList.Count; i++)
            {
                DAL.Models.Table? table = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == orderDetailsvm.tableList[i].TableId && !t.Isdelete);
                table.Status = "Available";
                table.ModifiedAt = DateTime.Now;
                table.ModifiedBy=userId;
                _context.Update(table);
            }

            List<Kot> kotList = _context.Kots.Where(kot => kot.OrderId == orderDetailsvm.OrderId && !kot.Isdelete).ToList();
                foreach (var kot in kotList)
                {
                    kot.Isdelete = true;
                    kot.ModifiedAt=DateTime.Now;
                    kot.ModifiedBy=userId;
                    _context.Update(kot);
                }


            await _context.SaveChangesAsync();

            //add tax in taxinvoicemapping table
            for (int i = 0; i < orderDetailsvm.TaxesInOrderDetails.Count; i++)
            {
                Taxinvoicemapping taxinvoicemapping = new();
                taxinvoicemapping.TaxId = orderDetailsvm.TaxesInOrderDetails[i].TaxId;
                taxinvoicemapping.InvoiceId = orderDetailsvm.InvoiceId;
                taxinvoicemapping.TaxName = orderDetailsvm.TaxesInOrderDetails[i].TaxName;
                taxinvoicemapping.TaxAmount = orderDetailsvm.TaxesInOrderDetails[i].TaxValue;
                await _context.AddAsync(taxinvoicemapping);
            }

            await _context.SaveChangesAsync();
            return orderDetailsvm;
        }
        catch (Exception e)
        {
            return null;
        }

    }
    #endregion

    #region IsAllItemReady
    public async Task<bool> IsAllItemReady(List<int> orderDetailId, OrderDetaIlsInvoiceViewModel orderDetailsvm)
    {
        foreach (int od in orderDetailId)
        {
            Orderdetail? orderdetail = await _context.Orderdetails.FirstOrDefaultAsync(x => x.OrderdetailId == od);
            if (orderdetail.Quantity != orderdetail.ReadyQuantity)
            {
                return false;
            }
        }
        return true;
    }
    #endregion

    #region IsAnyItemReady
    public bool IsAnyItemReady(OrderDetaIlsInvoiceViewModel orderDetailsvm)
    {
        if(orderDetailsvm.OrderId ==0){return false;}
        for (int i = 0; i < orderDetailsvm.ItemsInOrderDetails.Count; i++)
        {
            if (orderDetailsvm.ItemsInOrderDetails[i].OrderDetailId != 0)
            {
                Orderdetail? orderdetail = _context.Orderdetails.FirstOrDefault(x => x.OrderdetailId == orderDetailsvm.ItemsInOrderDetails[i].OrderDetailId);
                if (orderdetail.ReadyQuantity > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region isItemRedy
    public bool IsItemReady(long orderDetailId)
    {
        Orderdetail? orderdetail = _context.Orderdetails.FirstOrDefault(x => x.OrderdetailId == orderDetailId);
        if (orderdetail.Quantity != orderdetail.ReadyQuantity)
        {
            return false;
        }
        return true;
    }
    #endregion

    #region CancelOrder
    public async Task<bool> CancelOrder(OrderDetaIlsInvoiceViewModel orderDetailsvm, long userId)
    {
        try
        {
            // if (orderDetailsvm.InvoiceId != 0)
            // {
            //     Invoice? invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == orderDetailsvm.InvoiceId && !i.Isdelete);
            //     invoice.Isdelete = true;
            //     _context.Update(invoice);

            // }
            if (orderDetailsvm.OrderId != 0)
            {
                Order? order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderDetailsvm.OrderId && !o.Isdelete);
                order.Status = "Cancelled";
                order.TotalAmount = 0;
                order.ModifiedAt=DateTime.Now;
                order.ModifiedBy=userId;
                // order.OtherInstruction = null;
                _context.Update(order);

                List<Kot> kotList = _context.Kots.Where(kot => kot.OrderId == orderDetailsvm.OrderId && !kot.Isdelete).ToList();
                foreach (var kot in kotList)
                {
                    kot.Isdelete = true;
                    kot.ModifiedAt=DateTime.Now;
                    kot.ModifiedBy=userId;
                    _context.Update(kot);
                }
            }

            for (int i = 0; i < (orderDetailsvm.ItemsInOrderDetails == null ? 0 : orderDetailsvm.ItemsInOrderDetails.Count); i++)
            {
                if (orderDetailsvm.ItemsInOrderDetails[i].OrderDetailId != 0)
                {
                    Orderdetail? orderdetail = await _context.Orderdetails.FirstOrDefaultAsync(od => od.OrderdetailId == orderDetailsvm.ItemsInOrderDetails[i].OrderDetailId && !od.Isdelete);
                    // orderdetail.Isdelete = true;
                    orderdetail.Status = "Cancelled";
                    orderdetail.ModifiedAt = DateTime.Now;
                    orderdetail.ModifiedBy=userId;
                    _context.Update(orderdetail);

                    // List<Modifierorder> modifierorderList = _context.Modifierorders.Where(mo => mo.OrderdetailId == orderDetailsvm.ItemsInOrderDetails[i].OrderDetailId && !mo.Isdelete).ToList();
                    // foreach (var modiferorder in modifierorderList)
                    // {
                    //     modiferorder.Isdelete = true;
                    //     _context.Update(modiferorder);
                    // }
                }
            }

            List<Assigntable> assigntableList = _context.Assigntables.Where(at => at.CustomerId == orderDetailsvm.CustomerId && !at.Isdelete).ToList();
            foreach (var table in assigntableList)
            {
                table.Isdelete = true;
                table.ModifiedAt=DateTime.Now;
                table.ModifiedBy=userId;
                _context.Update(table);

                DAL.Models.Table? table1 = await _context.Tables.FirstOrDefaultAsync(t => t.TableId == table.TableId && !t.Isdelete);
                table1.Status = "Available";
                table1.ModifiedAt=DateTime.Now;
                table1.ModifiedBy=userId;
                _context.Update(table1);

            }

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
