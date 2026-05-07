using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleParts.API.Models;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }

    [Column("fullname")]
    public string FullName { get; set; } = "";

    [Column("email")]
    public string Email { get; set; } = "";

    [Column("password")]
    public string Password { get; set; } = "";

    [Column("role")]
    public string Role { get; set; } = "";
}