namespace VehicleParts.API.Models;

public class ServiceReview
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
}