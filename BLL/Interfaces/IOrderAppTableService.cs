using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppTableService
{
        public List<SectionViewModelForOrderAppTable> GetSectionList();

        public List<TableViewModelForOrderAppTable> GetTableDetailsBySection(long SectionId);

        Task<bool> AddCustomer(WaitingTokenDetailsViewModel waitingTokenvm, long userId);

        public long IsCustomerPresent(string Email);

        Task<bool> AddCustomerToWaitingList(WaitingTokenDetailsViewModel waitingTokenvm, long userId);
}
