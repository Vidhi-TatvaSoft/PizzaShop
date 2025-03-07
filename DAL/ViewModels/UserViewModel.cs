using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DAL.ViewModels;

public class UserViewModel
{
    public long UserId { get; set; }

    public long UserloginId { get; set; }

    public long RoleId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required and should not include whitespace")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "FirstName must contain only alphabets")]
    [StringLength(20, ErrorMessage = "First Name cannot exceed 20 characters.")]

    public string FirstName { get; set; } = null!;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required and should not include whitespace")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "LastName must contain only alphabets")]
    [StringLength(20, ErrorMessage = "Last Name cannot exceed 20 characters.")]

    public string LastName { get; set; } = null!;

    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required and should not include whitespace")]
    [StringLength(30, ErrorMessage = "user Name cannot exceed 30 characters.")]

    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [MaxLength(16, ErrorMessage = "Password cannot exceed 16 characters.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
              ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public bool? Status { get; set; }
    public string Image { get; set; }

    public IFormFile ProfileImage { get; set; }

    public long? CountryId { get; set; }

    public long? StateId { get; set; }

    public long? CityId { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Address should not include whitespace")]
    public string? Address { get; set; }


    [Required(ErrorMessage = "Zipcode is required.")]
    [Range(100000, 999999, ErrorMessage = "Zipcode must be 6 digits long.")]
    public long? Zipcode { get; set; }

    [Required(ErrorMessage = "phone number is required.")]
    [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be 10 digits long.")]
    public int Phone { get; set; }
}
