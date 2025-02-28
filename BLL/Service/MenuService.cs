using BLL.Interfaces;
using DAL.Models;

namespace BLL.Service;

public class MenuService: IMenuService
{
    private readonly PizzashopDbContext _context;

    public MenuService(PizzashopDbContext context)
    {
        _context = context;
    }

    
}
