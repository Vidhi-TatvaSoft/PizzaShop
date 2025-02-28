using BLL.Helpers;
using DAL.Models;
using DAL.ViewModels;

namespace BLL.Service.Interfaces;

public interface UserInterface
{

    List<User> getUserFromEmail(string token);
    
    public List<User> getUserFromEmailWithoutToken(string Email);

    public bool UpdateProfile(UserViewModel user, string Email);

    bool ChangepasswordService(ChangePasswordViewModel changePassword, string Email);

    public List<Country> GetCountry();

    public List<State> GetState(long? countryId);

    public List<City> GetCity(long? StateId);


    // public List<User> getuser(string Email);

    public PaginationHelper<User> GetUserList(string search = "", string sortColumn = "Name", string sortDirection = "asc", int pageNumber = 1, int pageSize = 5);

    public  Task<bool> EditUser(UserViewModel userVM , String Email);

}
