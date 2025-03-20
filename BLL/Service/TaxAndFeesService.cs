using System.Threading.Tasks;
using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class TaxAndFeesService : ITaxAndFees
{
    private readonly PizzashopDbContext _context;

    public TaxAndFeesService(PizzashopDbContext context)
    {
        _context = context;
    }

    // public async Task<List<Tax>> GetTaxes()
    // {
    //     List<Tax> allTax = _context.Taxes.Where(x => x.Isdelete == false).OrderBy(x => x.TaxId).ToList();
    //     return allTax;
    // }

    #region get all tax paginated
    public PaginationViewModel<Tax> GetTaxes(string search = "", int pageNumber = 1, int pageSize = 5)
    {

        var query = _context.Taxes.Where(u => u.Isdelete == false)
            .AsQueryable();

        //search 
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();
            query = query.Where(u =>
                u.TaxName.ToLower().Contains(lowerSearchTerm)
            );
        }

        // Get total records count (before pagination)
        int totalCount = query.Count();

        // Apply pagination
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<Tax>(items, totalCount, pageNumber, pageSize);
    }
    #endregion

    #region GetTaxByName
    public async Task<Tax> GetTaxByName(TaxViewModel taxvm){
        var presentName= await _context.Taxes.FirstOrDefaultAsync(x=>x.TaxName.ToLower().Trim()==taxvm.TaxName.ToLower().Trim() && x.Isdelete==false);
        if(presentName==null){return null;}
        else{return presentName;}
    }
    #endregion

    #region AddTax
    public async Task<bool> AddTax(TaxViewModel taxvm, long userID){
        if(taxvm==null){return false;}
        Tax tax = new();
        tax.TaxName=taxvm.TaxName;
        tax.TaxType=taxvm.TaxType;
        tax.TaxValue=taxvm.TaxValue;
        tax.Isenable=taxvm.Isenable;
        tax.Isdefault=taxvm.Isdefault;
        tax.CreatedBy=userID;

        await _context.AddAsync(tax);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region GetTaxDetailsById
    public async Task<TaxViewModel> GetTaxDetailsById(long taxID){
        Tax tax =await _context.Taxes.FirstOrDefaultAsync(x=>x.TaxId==taxID&& x.Isdelete==false);
        if(tax==null){return null;}
        TaxViewModel taxvm = new();
        taxvm.TaxId=tax.TaxId;
        taxvm.TaxName=tax.TaxName;
        taxvm.TaxType=tax.TaxType;
        taxvm.TaxValue=tax.TaxValue;
        taxvm.Isdefault=tax.Isdefault;
        taxvm.Isenable=(bool)tax.Isenable;
        return taxvm;
    }
    #endregion

    #region  GetTaxByNameForEdit
    public async Task<Tax> GetTaxByNameForEdit(TaxViewModel taxvm){
        var presentName=await  _context.Taxes.FirstOrDefaultAsync(x=>x.TaxName.ToLower().Trim()==taxvm.TaxName.ToLower().Trim() && x.TaxId!=taxvm.TaxId && x.Isdelete==false);
        if(presentName==null){return null;}
        else{return presentName;}
    }
    #endregion

    #region EditTax
    public async Task<bool> EditTax(TaxViewModel taxvm,long userID){
        if(taxvm==null){return false;}
        Tax tax =await _context.Taxes.FirstOrDefaultAsync(x=>x.TaxId==taxvm.TaxId && x.Isdelete==false);
        tax.TaxName=taxvm.TaxName;
        tax.TaxType=taxvm.TaxType;
        tax.TaxValue=taxvm.TaxValue;
        tax.Isenable=taxvm.Isenable;
        tax.Isdefault=taxvm.Isdefault;
        tax.ModifiedBy=userID;
        tax.ModifiedAt=DateTime.Now;

         _context.Taxes.Update(tax);
        await _context.SaveChangesAsync();
        return true;
    }

    #endregion

    #region DeleteTax
    public async Task<bool> DeleteTax(long id){
         Tax tax =await _context.Taxes.FirstOrDefaultAsync(x=>x.TaxId==id && x.Isdelete==false);
        if(tax==null){return false;}
        tax.Isdelete=true;
        _context.Taxes.Update(tax);
        await _context.SaveChangesAsync();
        return true;
    }
    #endregion

    



}
