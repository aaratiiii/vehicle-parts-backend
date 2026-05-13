namespace VehicleParts.API.Models;

public class UnavailablePartRequest
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string PartName { get; set; } = "";
    public string VehicleModel { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
}