using DAL.Models;
using DAL.ViewModels;

namespace BLL.Service.Interfaces;

public interface UserInterface
{

    List<User> getUserFromEmail(string token);
    
    bool UpdateUser(User user, string Email);

    bool ChangepasswordService(ChangePasswordViewModel changePassword, string Email);

    public List<Country> GetCountry();

    public List<State> GetState(long? countryId);

    public List<City> GetCity(long? StateId);

    public List<User> getuser(string Email);

}
