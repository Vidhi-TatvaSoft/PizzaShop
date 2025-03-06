using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class MenuService : IMenuService
{
    private readonly PizzashopDbContext _context;

    public MenuService(PizzashopDbContext context)
    {
        _context = context;
    }

    public List<Category> GetAllCategories()
    {
        return _context.Categories.Where(x => x.Isdelete == false).ToList();
    }



    public async Task<bool> AddCategory(Category category, long userId)
    {
        var ispresentcat = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryName == category.CategoryName);
        if (ispresentcat != null)
        {
            return false;
        }
        if (category == null)
        {
            return false;
        }
        Category cat = new Category();
        cat.CategoryName = category.CategoryName;
        cat.Description = category.Description;
        cat.CreatedBy = userId;
        await _context.Categories.AddAsync(cat);
        await _context.SaveChangesAsync();
        return true;

    }

    public async Task<bool> EditCategory(Category category, long catID, long userId)
    {
        if (category == null || catID == null)
        {
            return false;
        }
        Category cat = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == catID);
        cat.CategoryName = category.CategoryName;
        cat.Description = category.Description;
        cat.ModifiedBy = userId;
        cat.ModifiedAt = DateTime.Now;
        _context.Update(cat);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategory(long catID)
    {
        if (catID == null)
        {
            return false;
        }
        Category category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == catID);
        category.Isdelete = true;
        _context.Update(category);
        await _context.SaveChangesAsync();
        return true;


    }


    // items
    // public List<Item> GetItemsByCategory(long catID)
    // {
    //     return _context.Items.Where(x=>x.CategoryId == catID).ToList();
    // }

    public PaginationViewModel<ItemsViewModel> GetItemsByCategory(long? catID, string search = "", int pageNumber = 1, int pageSize = 5)
    {

        var query = _context.Items
            .Include(x => x.Category).Include(x => x.ItemType)
            .Where(x => x.CategoryId == catID).Where(x => x.Isdelete == false)
            .Select(x => new ItemsViewModel
            {
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                CategoryId = x.CategoryId,
                ItemTypeId = x.ItemTypeId,
                TypeImage = x.ItemType.TypeImage,
                Rate = x.Rate,
                Quantity = x.Quantity,
                ItemImage = x.ItemImage,
                Isavailable = x.Isavailable,
                Isdelete = x.Isdelete
            })
            .AsQueryable();

        //search 
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();
            query = query.Where(x =>
                x.ItemName.ToLower().Contains(lowerSearchTerm)
            );
        }

        // Get total records count (before pagination)
        int totalCount = query.Count();

        // Apply pagination
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<ItemsViewModel>(items, totalCount, pageNumber, pageSize);
    }


    public async Task<bool> AddItem(AddItemViewModel addItemvm, long userId)
    {
        if (addItemvm.CategoryId == null)
        {
            return false;
        }
        Item item = new();
        item.CategoryId = addItemvm.CategoryId;
        item.ItemName = addItemvm.ItemName;
        item.ItemTypeId = addItemvm.ItemTypeId;
        item.Rate = addItemvm.Rate;
        item.Quantity = addItemvm.Quantity;
        item.Unit = addItemvm.Unit;
        item.Isavailable = addItemvm.Isavailable;
        item.Isdefaulttax = addItemvm.Isdefaulttax;
        item.TaxValue = addItemvm.TaxValue;
        item.ShortCode = addItemvm.ShortCode;
        item.Description = addItemvm.Description;
        item.ItemImage = addItemvm.ItemImage;
        item.CreatedBy = userId;

        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItem(long itemID)
    {
        if (itemID == null)
        {
            return false;
        }
        Item item = await _context.Items.FirstOrDefaultAsync(x => x.ItemId == itemID);
        item.Isdelete = true;
        _context.Update(item);
        await _context.SaveChangesAsync();
        return true;
    }


}
