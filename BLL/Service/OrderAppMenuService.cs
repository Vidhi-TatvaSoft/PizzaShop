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
    public List<ItemsViewModel> GetItemByCategory(long categoryId, string searchText = ""){
        var allItems = _context.Items.Where(x => x.Isavailable == true && x.Isdelete == false).ToList(); 

        if(categoryId == -1){
            allItems = allItems.Where(x => x.IsFavourite == true).ToList();
        }else if(categoryId == 0){
            allItems = allItems;
        }else{
            allItems = allItems.Where(x => x.CategoryId == categoryId).ToList();
        }

        if(!searchText.IsNullOrEmpty()){
            allItems = allItems.Where(x => x.ItemName.ToLower().Trim().Contains(searchText.ToLower().Trim())).ToList();
        }

        List<ItemsViewModel> itemsList = allItems.Select(i => new ItemsViewModel{
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
    public async Task<bool> FavouriteItemManage(long itemId, bool IsFavourite){
        Item? item =await _context.Items.FirstOrDefaultAsync(x => x.ItemId == itemId && x.Isdelete== false);
        if(item != null){
            item.IsFavourite = IsFavourite;
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }else{
            return false;
        }
    }
    #endregion

    #region GetModifiersByItemId
    public List<ModifierGroupForItem> GetModifiersByItemId(long itemId){
        Item? itemdata = _context.Items.Include(x => x.Itemmodifiergroupmappings).ThenInclude(x=>x.ModifierGrp).ThenInclude(x => x.Modifiers)
                        .FirstOrDefault(x => x.ItemId == itemId && x.Isdelete == false);
        long typeId = itemdata.ItemTypeId;
        if(itemdata == null){
            return new List<ModifierGroupForItem>();
        }else{
            List<ModifierGroupForItem> modifierGrpList = itemdata.Itemmodifiergroupmappings.Where(mg =>  mg.Isdelete==false).Select(mg => new ModifierGroupForItem{
                ModifierGrpId = (long)mg.ModifierGrpId,
                ModifierGrpName = mg.ModifierGrp.ModifierGrpName,
                min = mg.Minvalue,
                max = mg.Maxvalue,
                TypeId = typeId,
                modifierList = mg.ModifierGrp.Modifiers.Where(m => m.Isdelete==false).Select(m => new Modifier{
                    ModifierId = m.ModifierId,
                    ModifierName = m.ModifierName,
                    Rate = m.Rate
                }).ToList()
            }).ToList();
            return modifierGrpList;
        }
    }
    #endregion

}
