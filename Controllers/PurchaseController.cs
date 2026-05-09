using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleParts.API.Data;
using VehicleParts.API.Models;

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

    [HttpGet]
    public IActionResult GetPurchaseInvoices()
    {
        var invoices = _context.PurchaseInvoices
            .Include(i => i.Items)
            .OrderByDescending(i => i.Id)
            .Select(i => new
            {
                i.Id,
                i.InvoiceNumber,
                i.VendorName,
                i.PurchaseDate,
                i.PaymentStatus,
                i.Note,
                i.TotalAmount,
                Items = i.Items.Select(item => new
                {
                    item.Id,
                    item.PartId,
                    item.PartName,
                    item.PartNumber,
                    item.Quantity,
                    item.UnitPrice,
                    item.LineTotal
                }).ToList()
            })
            .ToList();

        return Ok(invoices);
    }

    [HttpPost]
    public IActionResult CreatePurchase([FromBody] PurchaseRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            return BadRequest(new { message = "Please add at least one part." });

        decimal totalAmount = 0;
        var invoiceItems = new List<PurchaseInvoiceItem>();

        foreach (var item in request.Items)
        {
            var part = _context.Parts.FirstOrDefault(p => p.Id == item.PartId);

            if (part == null)
                return BadRequest(new { message = "Part not found." });

            var lineTotal = item.Quantity * item.UnitPrice;
            totalAmount += lineTotal;

            part.StockQuantity += item.Quantity;

            invoiceItems.Add(new PurchaseInvoiceItem
            {
                PartId = part.Id,
                PartName = part.Name,
                PartNumber = part.PartNumber,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = lineTotal
            });
        }

        var invoice = new PurchaseInvoice
        {
            InvoiceNumber = request.InvoiceNumber,
            VendorName = request.VendorName,
            PurchaseDate = request.PurchaseDate,
            PaymentStatus = request.PaymentStatus,
            Note = request.Note,
            TotalAmount = totalAmount,
            Items = invoiceItems
        };

        _context.PurchaseInvoices.Add(invoice);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Purchase invoice created, saved, and stock updated successfully.",
            invoiceId = invoice.Id,
            invoiceNumber = invoice.InvoiceNumber,
            totalAmount = invoice.TotalAmount
        });
    }
}

public class PurchaseRequest
{
    public string InvoiceNumber { get; set; } = "";
    public string VendorName { get; set; } = "";
    public DateTime PurchaseDate { get; set; }
    public string PaymentStatus { get; set; } = "Paid";
    public string Note { get; set; } = "";
    public List<PurchaseItem> Items { get; set; } = new();
}

public class PurchaseItem
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}