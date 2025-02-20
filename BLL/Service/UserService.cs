using System.Threading.Tasks;
using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class UserService
{
    private readonly PizzashopDbContext _context;
        private readonly JWTTokenService _jwttokenService;

        public UserService(PizzashopDbContext context, JWTTokenService jwttokenService)
        {
            _context = context;
            _jwttokenService = jwttokenService;
        }



    //  public async Task<User> getuser(string Email)
    //     {
    //         return _context.Users.Include(x => x.Userlogin).FirstOrDefault(x => x.Userlogin.Email == Email);
    //         // .Include(x=> x.Role).Include(x=>x.Country)
    //         //                     .Include(x=>x.State).Include(x=>x.City).
    //     }

        public List<User> getUserFromEmail(string token)
       {
            
            var claims = _jwttokenService.GetClaimsFromToken(token);
            var Email = _jwttokenService.GetClaimValue(token, "email");
            return _context.Users.Include(x => x.Userlogin).Where(x => x.Userlogin.Email == Email).ToList();
       }

       public Task<User> UpdateUser(User user)
       {
           _context.Users.Update(user);
           _context.SaveChanges();
           return Task.FromResult(user);
       }
}
