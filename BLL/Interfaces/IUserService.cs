using DAL.Models;
using DAL.ViewModels;

namespace BLL.Service.Interfaces;

public interface IUserService
{

    List<User> getUserFromEmail(string token);
    
    List<User> getUserFromEmailWithoutToken(string Email);

    List<Role> GetRole();
    List<Country> GetCountry();
    List<State> GetState(long? countryId);
    List<City> GetCity(long? StateId);

    PaginationViewModel<User> GetUserList(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5);

     bool UpdateProfile(UserViewModel user, string Email);
     Task<bool> IsUserNameExists(string Username);
     bool IsUserNameExistsForEdit(string Username, string Email);
     Task<bool> AddUser(UserViewModel userVM, long userId);
     Task<bool> EditUser(UserViewModel userVM, String Email);
     Task<bool> deleteUser(string Email);
     bool ChangepasswordService(ChangePasswordViewModel changePassword, string Email);



}
