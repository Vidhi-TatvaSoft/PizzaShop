using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppWaitingService
{
        public List<OrderAppWLSectionViewModel> GetAllSection();
        public List<WaitingTokenDetailsViewModel> GetWaitingListBySection(long sectionId);

        public WaitingTokenDetailsViewModel GetWaitingTokenDetailsById(long waitingId);
        Task<bool> DeleteWaitingToken(long waitingId);

        public List<TableViewModel> GetTableBySection(long sectionID);

        public long GetCustmerIdByEmail(long waitingId);
        Task<bool> AssignTable(int[] TableIds, long waitingId, long sectionId, long userId);
}
