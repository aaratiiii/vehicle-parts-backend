namespace VehicleParts.API.Models;

public class SalesInvoice
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string VehicleNumber { get; set; } = "";
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public string PaymentStatus { get; set; } = "Paid";
    
    public string CustomerEmail { get; set; } = "";
    
    public bool EmailSent { get; set; } = false;

    public List<SalesInvoiceItem> Items { get; set; } = new();
}