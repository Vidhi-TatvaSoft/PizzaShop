using DAL.Models;
using DAL.ViewModels;

namespace BLL.Service.Interfaces;

public interface UserLoginInterface
{
    string EncryptPassword(string password);
    Task<List<Userlogin>> getusers();
    string VerifyUserPassword(UserLoginViewModel userlogin);
    bool CheckEmailExist(string email);
    bool ResetPassword(ResetPasswordViewModel resetpassdata);

    
    public string GetProfileImage(string Email);

}
