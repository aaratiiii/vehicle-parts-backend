namespace VehicleParts.API.Models;

public class Vendor
{
    public int Id { get; set; }
    public string VendorName { get; set; } = "";
    public string ContactPerson { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Address { get; set; } = "";
}