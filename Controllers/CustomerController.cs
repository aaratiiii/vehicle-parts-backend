using Microsoft.AspNetCore.Mvc;
using VehicleParts.API.Data;
using VehicleParts.API.Models;

namespace VehicleParts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult RegisterCustomer(CustomerRegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.PhoneNumber) ||
            string.IsNullOrWhiteSpace(request.VehicleNumber))
        {
            return BadRequest(new
            {
                message = "Please fill all required fields."
            });
        }

        var emailExists = _context.Users.Any(u => u.Email == request.Email);

        if (emailExists)
        {
            return BadRequest(new
            {
                message = "Email already exists."
            });
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password,
            Role = "Customer"
        };

        _context.Users.Add(user);

        var customer = new Customer
        {
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email
        };

        _context.Customers.Add(customer);
        _context.SaveChanges();

        var vehicle = new Vehicle
        {
            CustomerId = customer.Id,
            VehicleNumber = request.VehicleNumber,
            VehicleBrand = request.VehicleBrand,
            VehicleModel = request.VehicleModel
        };

        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Customer registered successfully.",
            customerId = customer.Id
        });
    }

    [HttpPost("book-appointment")]
    public IActionResult BookAppointment(Appointment appointment)
    {
        appointment.Status = "Pending";

        _context.Appointments.Add(appointment);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Appointment booked successfully."
        });
    }

    [HttpGet("appointments/{customerId}")]
    public IActionResult GetAppointments(int customerId)
    {
        var appointments = _context.Appointments
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.Id)
            .ToList();

        return Ok(appointments);
    }

    [HttpDelete("appointments/{id}")]
    public IActionResult DeleteAppointment(int id)
    {
        var appointment = _context.Appointments
            .FirstOrDefault(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound(new
            {
                message = "Appointment not found."
            });
        }

        _context.Appointments.Remove(appointment);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Appointment deleted successfully."
        });
    }

    [HttpGet("all-appointments")]
    public IActionResult GetAllAppointments()
    {
        var appointments = _context.Appointments
            .OrderByDescending(a => a.AppointmentDate)
            .Select(a => new
            {
                a.Id,
                a.CustomerId,

                CustomerName = _context.Customers
                    .Where(c => c.Id == a.CustomerId)
                    .Select(c => c.FullName)
                    .FirstOrDefault() ?? "Unknown Customer",

                CustomerPhone = _context.Customers
                    .Where(c => c.Id == a.CustomerId)
                    .Select(c => c.PhoneNumber)
                    .FirstOrDefault() ?? "N/A",

                a.ServiceType,
                a.AppointmentDate,
                a.Status,
                a.Notes
            })
            .ToList();

        return Ok(appointments);
    }

    [HttpPost("request-part")]
    public IActionResult RequestUnavailablePart(UnavailablePartRequest request)
    {
        request.Status = "Pending";
        request.RequestedDate = DateTime.UtcNow;

        _context.UnavailablePartRequests.Add(request);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Unavailable part request submitted successfully."
        });
    }

    [HttpGet("part-requests/{customerId}")]
    public IActionResult GetPartRequests(int customerId)
    {
        var requests = _context.UnavailablePartRequests
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.Id)
            .ToList();

        return Ok(requests);
    }

    [HttpGet("all-part-requests")]
    public IActionResult GetAllPartRequests()
    {
        var requests = _context.UnavailablePartRequests
            .OrderByDescending(r => r.RequestedDate)
            .Select(r => new
            {
                r.Id,
                r.CustomerId,

                CustomerName = _context.Customers
                    .Where(c => c.Id == r.CustomerId)
                    .Select(c => c.FullName)
                    .FirstOrDefault() ?? "Unknown Customer",

                CustomerPhone = _context.Customers
                    .Where(c => c.Id == r.CustomerId)
                    .Select(c => c.PhoneNumber)
                    .FirstOrDefault() ?? "N/A",

                r.PartName,
                r.VehicleModel,
                r.Description,
                r.Status,
                r.RequestedDate
            })
            .ToList();

        return Ok(requests);
    }

    [HttpPost("review-service")]
    public IActionResult ReviewService(ServiceReview review)
    {
        if (review.Rating < 1 || review.Rating > 5)
        {
            return BadRequest(new
            {
                message = "Rating must be between 1 and 5."
            });
        }

        review.ReviewDate = DateTime.UtcNow;

        _context.ServiceReviews.Add(review);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Service review submitted successfully."
        });
    }

    [HttpGet("reviews")]
    public IActionResult GetReviews()
    {
        var reviews = _context.ServiceReviews
            .OrderByDescending(r => r.ReviewDate)
            .Select(r => new
            {
                r.Id,
                r.CustomerId,

                CustomerName = _context.Customers
                    .Where(c => c.Id == r.CustomerId)
                    .Select(c => c.FullName)
                    .FirstOrDefault() ?? "Unknown Customer",

                CustomerPhone = _context.Customers
                    .Where(c => c.Id == r.CustomerId)
                    .Select(c => c.PhoneNumber)
                    .FirstOrDefault() ?? "N/A",

                r.Rating,
                r.Comment,
                r.ReviewDate
            })
            .ToList();

        return Ok(reviews);
    }

    [HttpGet("history/{customerId}")]
    public IActionResult GetPurchaseServiceHistory(int customerId)
    {
        var customer = _context.Customers
            .FirstOrDefault(c => c.Id == customerId);

        if (customer == null)
        {
            return NotFound(new
            {
                message = "Customer not found."
            });
        }

        var appointments = _context.Appointments
            .Where(a => a.CustomerId == customerId)
            .OrderByDescending(a => a.Id)
            .ToList();

        var invoices = _context.SalesInvoices
            .Where(i =>
                i.CustomerPhone == customer.PhoneNumber ||
                i.CustomerEmail == customer.Email)
            .OrderByDescending(i => i.Id)
            .ToList();

        return Ok(new
        {
            purchases = invoices,
            services = appointments
        });
    }
}

public class CustomerRegisterRequest
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Password { get; set; } = "";
    public string VehicleNumber { get; set; } = "";
    public string VehicleBrand { get; set; } = "";
    public string VehicleModel { get; set; } = "";
}