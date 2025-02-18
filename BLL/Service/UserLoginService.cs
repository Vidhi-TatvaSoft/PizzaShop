using DAL.Models;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class UserLoginService
    {
        private readonly PizzashopDbContext _context;

        public UserLoginService(PizzashopDbContext context)
        {
            _context = context;
        }

        public async Task<List<Userlogin>> getusers()
        {
            var pizzashopDbContext = _context.Userlogins.Include(u => u.Role);
            return await pizzashopDbContext.ToListAsync();
        }

        public bool VerifyUserPassword(UserLoginViewModel userlogin)
        {
            if (_context.Userlogins.FirstOrDefault(e => e.Email == userlogin.Email && e.Password == userlogin.Password) != null)
            {
                return true;
            }
            return false;
        }

        public bool ResetPassword(ResetPasswordViewModel resetpassdata)
        {
            if (_context.Userlogins.FirstOrDefault(e => e.Email == resetpassdata.Email) != null)
            {
                Userlogin user = _context.Userlogins.FirstOrDefault(e => e.Email == resetpassdata.Email);
                user.Password = resetpassdata.Password;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }

}