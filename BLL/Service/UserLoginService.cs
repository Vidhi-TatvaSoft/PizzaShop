using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using BLL.Interfaces;
using NuGet.ProjectModel;
using System.Security.Cryptography.X509Certificates;

namespace BLL.Services
{
    public class UserLoginService : IUserLoginService
    {
        private readonly PizzashopDbContext _context;
        private readonly IJWTTokenService _jwttokenService;

        public UserLoginService(PizzashopDbContext context, IJWTTokenService jwttokenService)
        {
            _context = context;
            _jwttokenService = jwttokenService;
        }


        public string EncryptPassword(string password)
        {
            // byte[] passwordBytes = ASCIIEncoding.ASCII.GetBytes(password);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[0],
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public async Task<List<Userlogin>> getusers()
        {
            var pizzashopDbContext = _context.Userlogins.Include(u => u.Role);
            return await pizzashopDbContext.ToListAsync();
        }

        public string VerifyUserPassword(UserLoginViewModel userlogin)
        {
            try
            {
                var user = _context.Userlogins.Include(x => x.Users).FirstOrDefault(e => e.Email == userlogin.Email);

                if (user != null)
                {
                    if (user.Password == EncryptPassword(userlogin.Password) && user.IsDelete == false && user.Users.ToList()[0].Status == true)
                    {
                        var RoleObject = _context.Roles.Where(e => e.RoleId == user.RoleId).FirstOrDefault();
                        var token = _jwttokenService.GenerateToken(userlogin.Email, RoleObject.RoleName);
                        return token;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public bool CheckEmailExist(string email)
        {
            // if (_context.Userlogins.FirstOrDefault(e => e.Email == email && e.IsDelete==false) != null)
            // {
            //     return true;
            // }
            // return false;
            return _context.Userlogins.Any(e => e.Email == email && !e.IsDelete);
        }


        public string GetProfileImage(string Email)
        {
            return _context.Users.FirstOrDefault(x => x.Userlogin.Email == Email).ProfileImage;
        }

        public string GetUsername(string Email)
        {
            return _context.Users.FirstOrDefault(x => x.Userlogin.Email == Email).Username;
        }

        public long GetUserId(string Email)
        {
            return _context.Users.FirstOrDefault(x => x.Userlogin.Email == Email).UserId;
        }

        public string GetPassword(string Email)
        {
            return _context.Userlogins.FirstOrDefault(x => x.Email == Email).Password;
        }

        public bool ResetPassword(ResetPasswordViewModel resetpassdata)
        {
            if (_context.Userlogins.FirstOrDefault(e => e.Email == resetpassdata.Email && e.IsDelete == false) != null)
            {
                Userlogin user = _context.Userlogins.FirstOrDefault(e => e.Email == resetpassdata.Email && e.IsDelete == false);
                user.Password = EncryptPassword(resetpassdata.Password);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }

}