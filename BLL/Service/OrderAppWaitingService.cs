using BLL.Interfaces;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class OrderAppWaitingService : IOrderAppWaitingService
{
    private readonly PizzashopDbContext _context;

    public OrderAppWaitingService(PizzashopDbContext context)
    {
        _context = context;
    }

#region getAllsection
    public List<OrderAppWLSectionViewModel> GetAllSection(){
        return _context.Sections.Where(x => x.Isdelete == false )
                .Select(x => new OrderAppWLSectionViewModel{
                    SectionId = x.SectionId,
                    SectionName = x.SectionName,
                    WaitingCount = _context.Waitinglists.Count(w => w.SectionId == x.SectionId && w.Isassign==false && w.Isdelete == false)
                }).ToList();
    }
    #endregion


    #region GetWaitingListBySection
    public List<OrderAppWLListDetailsViewModel> GetWaitingListBySection(long sectionId){
        var data = _context.Waitinglists.Include(x => x.Customer).Where(x => x.Isassign==false && x.Isdelete == false).ToList();

        if(sectionId == 0){
            List<OrderAppWLListDetailsViewModel> waitinglist = data.
                                Select(w => new OrderAppWLListDetailsViewModel{
                                    WaitingId = w.WaitingId,
                                    CreatedAt = (DateTime)w.CreatedAt,
                                    Name = w.Customer.CustomerName,
                                    NoOfPerson = w.NoOfPerson,
                                    phoneno = (int)w.Customer.Phoneno,
                                    Email = w.Customer.Email
                                }).ToList();
            return waitinglist;
        }else{
            List<OrderAppWLListDetailsViewModel> waitinglist = data.Where(x => x.SectionId == sectionId)
                                .Select(w => new OrderAppWLListDetailsViewModel{
                                    WaitingId = w.WaitingId,
                                    CreatedAt = (DateTime)w.CreatedAt,
                                    Name = w.Customer.CustomerName,
                                    NoOfPerson = w.NoOfPerson,
                                    phoneno = (int)w.Customer.Phoneno,
                                    Email = w.Customer.Email
                                }).ToList();
            return waitinglist;
        }
    }
    #endregion

}
