using Microsoft.AspNetCore.Mvc;
using VehicleParts.API.Data;
using VehicleParts.API.Models;

namespace VehicleParts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StaffController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetStaff()
    {
        var staff = _context.Users
            .Where(u => u.Role == "Staff")
            .OrderByDescending(u => u.Id)
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Role
            })
            .ToList();

        return Ok(staff);
    }

    [HttpPost]
    public IActionResult AddStaff([FromBody] StaffRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Please fill all required fields." });
        }

        var emailExists = _context.Users.Any(u => u.Email == request.Email);

        if (emailExists)
            return BadRequest(new { message = "Email already exists." });

        var staff = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password,
            Role = "Staff"
        };

        _context.Users.Add(staff);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Staff registered successfully.",
            staff = new
            {
                staff.Id,
                staff.FullName,
                staff.Email,
                staff.Role
            }
        });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteStaff(int id)
    {
        var staff = _context.Users.FirstOrDefault(u => u.Id == id && u.Role == "Staff");

        if (staff == null)
            return NotFound(new { message = "Staff not found." });

        _context.Users.Remove(staff);
        _context.SaveChanges();

        return Ok(new { message = "Staff deleted successfully." });
    }

    [HttpPost("register-customer")]
    public IActionResult RegisterCustomer(RegisterCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.PhoneNumber) ||
            string.IsNullOrWhiteSpace(request.VehicleNumber))
        {
            return BadRequest(new { message = "Customer name, phone and vehicle number are required." });
        }

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
            VehicleNumber = request.VehicleNumber,
            VehicleBrand = request.VehicleBrand,
            VehicleModel = request.VehicleModel,
            CustomerId = customer.Id
        };

        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();

        return Ok(new { message = "Customer registered successfully." });
    }

    [HttpGet("customers")]
    public IActionResult GetCustomers()
    {
        var customers = _context.Customers
            .OrderByDescending(c => c.Id)
            .Select(c => new
            {
                c.Id,
                FullName = c.FullName,
                Name = c.FullName,
                PhoneNumber = c.PhoneNumber,
                Phone = c.PhoneNumber,
                c.Email,

                VehicleNumber = _context.Vehicles
                    .Where(v => v.CustomerId == c.Id)
                    .Select(v => v.VehicleNumber)
                    .FirstOrDefault(),

                RecentPurchase = _context.SalesInvoices
                    .Where(i => i.CustomerPhone == c.PhoneNumber)
                    .OrderByDescending(i => i.InvoiceDate)
                    .Select(i => i.Items.Any()
                        ? i.Items.Select(item => item.PartName).FirstOrDefault()
                        : "No Purchase")
                    .FirstOrDefault() ?? "No Purchase",

                CreditStatus = _context.SalesInvoices
                    .Any(i => i.CustomerPhone == c.PhoneNumber &&
                              i.PaymentStatus != "Paid")
                    ? "Pending"
                    : "Clear"
            })
            .ToList();

        return Ok(customers);
    }
}

public class StaffRequest
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class RegisterCustomerRequest
{
    public string FullName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Email { get; set; } = "";
    public string VehicleNumber { get; set; } = "";
    public string VehicleBrand { get; set; } = "";
    public string VehicleModel { get; set; } = "";
}