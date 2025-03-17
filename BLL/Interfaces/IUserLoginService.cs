using DAL.Models;
using DAL.ViewModels;

namespace BLL.Interfaces;

public interface IUserLoginService
{
    string EncryptPassword(string password);

    string Base64Encode(string plainText);
     string Base64Decode(string base64EncodedData);

    Task<List<Userlogin>> getusers();
    bool CheckEmailExist(string email);
    string GetProfileImage(string Email);
     string GetUsername(string Email);
     long GetUserId(string Email);
     string GetPassword(string Email);

     bool ResetPassword(ResetPasswordViewModel resetpassdata);
    string VerifyUserPassword(UserLoginViewModel userlogin);


}