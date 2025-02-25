using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class MyProfileViewModel
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Lastname is required.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public string Username { get; set; }


    [Required(ErrorMessage = "phone number is required.")]
    [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be 10 digits long.")]
    public int Phone { get; set; }

    public string CountryName { get; set; } = null!;
    public string StateName { get; set; } = null!;
    public string CityName { get; set; } = null!;

    public string? Address { get; set; } = null!;

    
    [Required(ErrorMessage = "Zipcode is required.")]
    [Range(100000, 999999, ErrorMessage = "Zipcode must be 6 digits long.")]
    public long? Zipcode { get; set; } = null!;
}
