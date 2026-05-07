namespace VehicleParts.API.Models;

public class Part
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string PartNumber { get; set; } = "";
    public string Category { get; set; } = "";
    public string VendorName { get; set; } = "";
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}