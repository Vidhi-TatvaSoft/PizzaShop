using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Current password is required.")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New password is required.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
    [MaxLength(100, ErrorMessage = "New password cannot exceed 100 characters.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
          ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
    public string NewPassword { get; set; }


    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "Confirm password does not match the new password.")]
    public string ConfirmPassword { get; set; }
}
