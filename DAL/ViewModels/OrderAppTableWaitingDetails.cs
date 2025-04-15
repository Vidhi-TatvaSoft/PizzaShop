namespace DAL.ViewModels;

public class OrderAppTableWaitingDetails
{
    public long ID{get;set;}
    public string Name{get;set;}
     public int NoOfPerson{get;set;}

     public WaitingTokenDetailsViewModel customerDetails{get;set;}
}
