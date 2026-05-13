namespace VehicleParts.API.Models;

public class Appointment
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string ServiceType { get; set; } = "";
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string Notes { get; set; } = "";
}