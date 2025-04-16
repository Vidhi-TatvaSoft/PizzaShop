using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppWaitingService
{
        public List<OrderAppWLSectionViewModel> GetAllSection();
        public List<OrderAppWLListDetailsViewModel> GetWaitingListBySection(long sectionId);
}
