using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class MyProfileViewModel
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required and should not include whitespace")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "FirstName must contain only alphabets")]
    [StringLength(20, ErrorMessage = "First Name cannot exceed 20 characters.")]
    public string FirstName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required and should not include whitespace")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Last Name must contain only alphabets")]
    [StringLength(20, ErrorMessage = "Last Name cannot exceed 20 characters.")]
    public string LastName { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "User name is required and should not include whitespace")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "UserName must contain only alphabets")]
    [StringLength(30, ErrorMessage = "User Name cannot exceed 30 characters.")]
    public string Username { get; set; }


    [Required(ErrorMessage = "phone number is required.")]
    [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be 10 digits long.")]
    public int Phone { get; set; }

    public string CountryName { get; set; } = null!;
    public string StateName { get; set; } = null!;
    public string CityName { get; set; } = null!;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Address is required and should not include whitespace")]
    public string? Address { get; set; } = null!;


    [Required(ErrorMessage = "Zipcode is required.")]
    [Range(100000, 999999, ErrorMessage = "Zipcode must be 6 digits long.")]
    public long? Zipcode { get; set; } = null!;
}
