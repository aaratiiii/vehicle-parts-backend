using Microsoft.AspNetCore.Mvc;
using VehicleParts.API.Data;
using VehicleParts.API.Models;

namespace VehicleParts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = _context.Users.FirstOrDefault(u =>
            u.Email == request.Email &&
            u.Password == request.Password &&
            u.Role == request.Role
        );

        if (user == null)
            return Unauthorized(new { message = "Invalid login" });

        return Ok(user);
    }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}