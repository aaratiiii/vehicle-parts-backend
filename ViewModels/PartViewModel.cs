namespace VehicleParts.API.ViewModels;

public class PartViewModel
{
    public string Name { get; set; } = "";
    public string PartNumber { get; set; } = "";
    public string Category { get; set; } = "";
    public string VendorName { get; set; } = "";
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}