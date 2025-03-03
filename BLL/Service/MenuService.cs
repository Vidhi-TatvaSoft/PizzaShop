using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class MenuService: IMenuService
{
    private readonly PizzashopDbContext _context;

    public MenuService(PizzashopDbContext context)
    {
        _context = context;
    }

    public List<Category> GetAllCategories()
    {
        return _context.Categories.Where(x=>x.Isdelete==false).ToList();
    }



    public async Task<bool> AddCategory(Category category)
    {
        if(category == null){
           return false;
        }
        Category cat = new Category();
        cat.CategoryName = category.CategoryName;
        cat.Description = category.Description;
        await _context.Categories.AddAsync(cat);
        await _context.SaveChangesAsync();
        return true;
        
    }

    public async Task<bool> EditCategory(Category category, long catID)
    {
        if(category == null || catID==null){
           return false;
        }
        Category cat =await  _context.Categories.FirstOrDefaultAsync(x=> x.CategoryId == catID);
        cat.CategoryName = category.CategoryName;
        cat.Description = category.Description;
         _context.Update(cat);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategory(long catID)
    {
        if(catID == null)
        {
            return false;
        }
        Category category =await _context.Categories.FirstOrDefaultAsync(x=>x.CategoryId == catID);
        category.Isdelete=true;
          _context.Update(category);
        await _context.SaveChangesAsync();
        return true;


    }

    


   
}
