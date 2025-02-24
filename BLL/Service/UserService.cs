using System.Threading.Tasks;
using BLL.Service.Interfaces;
using BLL.Services;
using DAL.Models;
using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service;

public class UserService : UserInterface
{
    private readonly PizzashopDbContext _context;
    private readonly JWTTokenService _jwttokenService;

    private readonly UserLoginService _userLoginService;

    public UserService(PizzashopDbContext context, JWTTokenService jwttokenService, UserLoginService userLoginService)
    {
        _context = context;
        _jwttokenService = jwttokenService;
        _userLoginService = userLoginService;
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

    public List<Role> GetRole()
    {
        return _context.Roles.ToList();
    }

    public List<Country> GetCountry()
    {
        return _context.Countries.ToList();
    }

    public List<State> GetState(long? countryId)
    {
        return _context.States.Where(x => x.CountryId == countryId).ToList();
    }

    public List<City> GetCity(long? StateId)
    {
        return _context.Cities.Where(x => x.StateId == StateId).ToList();
    }

    public List<User> getuser(string Email)
    {
        return _context.Users.Include(x => x.Userlogin).Include(x=>x.Role).ToList();
    }

    public bool UpdateUser(User user, string Email)
    {
        User userdetails = _context.Users.FirstOrDefault(x => x.Userlogin.Email == Email);
        userdetails.FirstName = user.FirstName;
        userdetails.LastName = user.LastName;
        userdetails.Username = user.Username;
        userdetails.Address = user.Address;
        userdetails.Phone = user.Phone;
        userdetails.Zipcode = user.Zipcode;
        userdetails.CountryId = user.CountryId;
        userdetails.StateId = user.StateId;
        userdetails.CityId = user.CityId;

        _context.Update(userdetails);
        _context.SaveChanges();
        return true;
    }

    public async Task<bool> AddUser(UserViewModel userVM , String Email)
    {
        if(_context.Userlogins.Any(x => x.Email == userVM.Email))
        {
            return false;
        }
        Userlogin userlogin = new Userlogin();
        userlogin.Email = userVM.Email;
        userlogin.Password = _userLoginService.EncryptPassword(userVM.Password);
        userlogin.RoleId = userVM.RoleId;

        await _context.AddAsync(userlogin);
        await _context.SaveChangesAsync();

        User user = new User();
        user.UserloginId = userlogin.UserloginId;
        user.FirstName = userVM.FirstName;
        user.LastName = userVM.LastName;
        user.Phone = userVM.Phone;
        user.Username = userVM.Username;
        user.RoleId = userVM.RoleId;

        user.ProfileImage = userVM.Image;
        // user.Status = userVM.Status;
        user.CountryId = userVM.CountryId;
        user.StateId = userVM.StateId;
        user.CityId = userVM.CityId;
        user.Address = userVM.Address;
        user.Zipcode = userVM.Zipcode;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return true;
    }


    public async Task<bool> EditUser(UserViewModel userVM , String Email)
    {
        if(_context.Userlogins.Any(x => x.Email == userVM.Email))
        {
            return false;
        }
        Userlogin userlogin = new Userlogin();
        userlogin.Email = userVM.Email;
        userlogin.Password = _userLoginService.EncryptPassword(userVM.Password);
        userlogin.RoleId = userVM.RoleId;

        await _context.AddAsync(userlogin);
        await _context.SaveChangesAsync();

        User user = new User();
        user.UserloginId = userlogin.UserloginId;
        user.FirstName = userVM.FirstName;
        user.LastName = userVM.LastName;
        user.Phone = userVM.Phone;
        user.Username = userVM.Username;
        user.RoleId = userVM.RoleId;

        user.ProfileImage = userVM.Image;
        // user.Status = userVM.Status;
        user.CountryId = userVM.CountryId;
        user.StateId = userVM.StateId;
        user.CityId = userVM.CityId;
        user.Address = userVM.Address;
        user.Zipcode = userVM.Zipcode;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public bool deleteUser(int id)
    {
        var user = _context.Users.FirstOrDefault(x => x.UserId == id).Isdelete = true;
        _context.SaveChanges();
        return true;
    }

    public bool ChangepasswordService(ChangePasswordViewModel changePassword, string Email)
    {
        var userdetails = _context.Users.Include(x => x.Userlogin).FirstOrDefault(x => x.Userlogin.Email == Email);
        var userpassword = userdetails.Userlogin.Password;
        if (userpassword == _userLoginService.EncryptPassword(changePassword.CurrentPassword))
        {
            userdetails.Userlogin.Password = _userLoginService.EncryptPassword(changePassword.NewPassword);
            _context.Update(userdetails);
            _context.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }


    }
}
