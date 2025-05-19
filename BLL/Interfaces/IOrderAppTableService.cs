using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IOrderAppTableService
{
        public List<SectionViewModelForOrderAppTable> GetSectionList();

        public List<TableViewModelForOrderAppTable> GetTableDetailsBySection(long SectionId);

        Task<bool> AddEditCustomer(WaitingTokenDetailsViewModel waitingTokenvm, long userId);

        Task<bool> IsCustomerPresentInWaiting(string Email);
        Task<bool> IsCustomerPresentInWaitingUpdate(string Email, long waitingId);

        public long IsCustomerPresent(string Email);
        public bool IsCustomerAlreadyAssigned(string Email);

        Task<bool> AddEditCustomerToWaitingList(WaitingTokenDetailsViewModel waitingTokenvm, long userId);
        
        public List<OrderAppTableWaitingDetails> GetListOfCustomerWaiting(long sectionId);
        Task<bool> Assigntable(string Email, int[] TableIds, long userId);
}
