using DAL.Models;

namespace DAL.ViewModels;

public class OrderDetaIlsInvoiceViewModel
{

    //order details

    public long InvoiceId { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public long OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; } = null!;



    //customer details
     public long CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public int? Phoneno { get; set; }

    public long NumberOfPerson{get;set;}

    public string? Email { get; set; }


    //table details
    public List<Table> tableList{get;set;}
    public long SectionId { get; set; }
    public string SectionName { get; set; } = null!;

    public List<ItemForInvoiceOrderDetails> ItemsInOrderDetails { get; set; }

    public List<TaxForOrderDetailsInvoice> TaxesInOrderDetails { get; set; }

    public decimal SubTotalAmountOfOrder { get; set; }

    public decimal TotalAmountOfOrderMain { get; set; }



    

    
}
