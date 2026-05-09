namespace VehicleParts.API.Models;

public class PurchaseInvoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public string VendorName { get; set; } = "";
    public DateTime PurchaseDate { get; set; }
    public string PaymentStatus { get; set; } = "Paid";
    public string Note { get; set; } = "";
    public decimal TotalAmount { get; set; }

    public List<PurchaseInvoiceItem> Items { get; set; } = new();
}