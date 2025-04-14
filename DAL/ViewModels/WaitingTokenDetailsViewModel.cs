using System.ComponentModel.DataAnnotations;

namespace DAL.ViewModels;

public class WaitingTokenDetailsViewModel
{
    public string Email {get;set;}

    public string Name{get; set;}

    public int Mobileno{get;set;}

    public int NoOfPerson{get;set;}

    public long SectionID{get; set;}

    public string SectionName{get;set;}


}
