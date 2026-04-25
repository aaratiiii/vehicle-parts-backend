namespace VehicleParts.API.Models;

public class Vehicle
{
    public int Id { get; set; }
    public string VehicleNumber { get; set; } = "";
    public string VehicleModel { get; set; } = "";
    public string VehicleBrand { get; set; } = "";

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
}