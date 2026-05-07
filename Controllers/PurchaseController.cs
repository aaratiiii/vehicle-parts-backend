using Microsoft.AspNetCore.Mvc;
using VehicleParts.API.Data;

namespace VehicleParts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PurchaseController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreatePurchase([FromBody] PurchaseRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            return BadRequest(new { message = "Please add at least one part." });

        foreach (var item in request.Items)
        {
            var part = _context.Parts.FirstOrDefault(p => p.Id == item.PartId);

            if (part == null)
                return BadRequest(new { message = $"Part not found." });

            part.StockQuantity += item.Quantity;
        }

        _context.SaveChanges();

        return Ok(new { message = "Purchase invoice created and stock updated successfully." });
    }
}

public class PurchaseRequest
{
    public string VendorName { get; set; } = "";
    public DateTime PurchaseDate { get; set; }
    public List<PurchaseItem> Items { get; set; } = new();
}

public class PurchaseItem
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}