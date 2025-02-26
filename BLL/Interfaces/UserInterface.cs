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

    public Task<(List<User>, int)> GetUsers(int PageNo, int PageSize);

    public  Task<bool> EditUser(UserViewModel userVM , String Email);

}
