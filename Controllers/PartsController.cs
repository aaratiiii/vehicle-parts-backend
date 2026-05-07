using Microsoft.AspNetCore.Mvc;
using VehicleParts.API.Data;
using VehicleParts.API.Models;
using VehicleParts.API.ViewModels;

namespace VehicleParts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PartsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetParts()
    {
        return Ok(_context.Parts.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetPartById(int id)
    {
        var part = _context.Parts.Find(id);

        if (part == null)
            return NotFound(new { message = "Part not found" });

        return Ok(part);
    }

    [HttpPost]
    public IActionResult AddPart(PartViewModel model)
    {
        var part = new Part
        {
            Name = model.Name,
            PartNumber = model.PartNumber,
            Category = model.Category,
            VendorName = model.VendorName,
            Price = model.Price,
            StockQuantity = model.StockQuantity
        };

        _context.Parts.Add(part);
        _context.SaveChanges();

        return Ok(part);
    }

    [HttpPut("{id}")]
    public IActionResult UpdatePart(int id, PartViewModel model)
    {
        var part = _context.Parts.Find(id);

        if (part == null)
            return NotFound(new { message = "Part not found" });

        part.Name = model.Name;
        part.PartNumber = model.PartNumber;
        part.Category = model.Category;
        part.VendorName = model.VendorName;
        part.Price = model.Price;
        part.StockQuantity = model.StockQuantity;

        _context.SaveChanges();

        return Ok(new { message = "Part updated successfully", data = part });
    }

    [HttpDelete("{id}")]
    public IActionResult DeletePart(int id)
    {
        var part = _context.Parts.Find(id);

        if (part == null)
            return NotFound(new { message = "Part not found" });

        _context.Parts.Remove(part);
        _context.SaveChanges();

        return Ok(new { message = "Part deleted successfully" });
    }
}