using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class WaitingTokenDetailsViewModel
{
    public long waitingId{get;set;}

    public DateTime CreatedAt{get;set;}

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    public string Email {get;set;}

    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required and should not include whitespace")]
    [RegularExpression(@"\S.*", ErrorMessage = "Only white space is not allowed")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string Name{get; set;}

    [Required(ErrorMessage = "Phone number is required.")]
    [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be 10 digits long.")]
    public long Mobileno{get;set;}

    [Required(ErrorMessage = "Number Of Person is required.")]
    [Range(0, 999, ErrorMessage = "Number Of Person Can not be negative and can not be greate than 1000")]
    public int NoOfPerson{get;set;}

    public long SectionID{get; set;}

    public string SectionName{get;set;}
}
