using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppWaitingService
{
        Task<List<OrderAppWLSectionViewModel>> GetAllSection();
        public List<WaitingTokenDetailsViewModel> GetWaitingListBySection(long sectionId);
        Task<List<WaitingTokenDetailsViewModel>> GetWaitingListBySectionSP(long sectionId);

        public WaitingTokenDetailsViewModel GetWaitingTokenDetailsById(long waitingId);
        Task<WaitingTokenDetailsViewModel> GetWaitingTokenDetailsByIdSP(long waitingId);
        Task<bool> DeleteWaitingToken(long waitingId,long userId);
        Task<bool> DeleteWaitingTokenSP(long waitingId, long userId);

        public List<TableViewModel> GetTableBySection(long sectionID);
        public List<TableViewModel> GetTableBySectionSP(long sectionID);

        public long GetCustmerIdByWaitingId(long waitingId);
        Task<bool> AssignTable(int[] TableIds, long waitingId, long sectionId, long userId);
}
