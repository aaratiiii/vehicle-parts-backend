using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleParts.API.Data;
using VehicleParts.API.Models;
using VehicleParts.API.Services;

namespace VehicleParts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesInvoiceController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;
    private readonly InvoicePdfService _invoicePdfService;

    public SalesInvoiceController(
        ApplicationDbContext context,
        EmailService emailService,
        InvoicePdfService invoicePdfService
    )
    {
        _context = context;
        _emailService = emailService;
        _invoicePdfService = invoicePdfService;
    }

    [HttpGet]
    public IActionResult GetSalesInvoices()
    {
        var invoices = _context.SalesInvoices
            .Include(i => i.Items)
            .OrderByDescending(i => i.Id)
            .Select(i => new
            {
                i.Id,
                i.CustomerName,
                i.CustomerPhone,
                i.CustomerEmail,
                i.VehicleNumber,
                i.InvoiceDate,
                i.TotalAmount,
                i.DiscountAmount,
                i.FinalAmount,
                i.PaymentStatus,
                i.EmailSent,
                Items = i.Items.Select(item => new
                {
                    item.Id,
                    item.PartId,
                    item.PartName,
                    item.Quantity,
                    item.UnitPrice,
                    item.LineTotal
                }).ToList()
            })
            .ToList();

        return Ok(invoices);
    }

    [HttpPost]
    public IActionResult CreateSalesInvoice(SalesInvoiceRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            return BadRequest(new { message = "Please add at least one part." });

        decimal totalAmount = 0;
        var invoiceItems = new List<SalesInvoiceItem>();

        foreach (var item in request.Items)
        {
            var part = _context.Parts.FirstOrDefault(p => p.Id == item.PartId);

            if (part == null)
                return BadRequest(new { message = "Part not found." });

            if (part.StockQuantity < item.Quantity)
                return BadRequest(new { message = $"Not enough stock for {part.Name}." });

            var lineTotal = part.Price * item.Quantity;
            totalAmount += lineTotal;

            part.StockQuantity -= item.Quantity;

            invoiceItems.Add(new SalesInvoiceItem
            {
                PartId = part.Id,
                PartName = part.Name,
                Quantity = item.Quantity,
                UnitPrice = part.Price,
                LineTotal = lineTotal
            });
        }

        var discount = totalAmount > 5000 ? totalAmount * 0.10m : 0;
        var finalAmount = totalAmount - discount;

        var invoice = new SalesInvoice
        {
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerEmail = request.CustomerEmail,
            VehicleNumber = request.VehicleNumber,
            InvoiceDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            DiscountAmount = discount,
            FinalAmount = finalAmount,
            PaymentStatus = request.PaymentStatus,
            EmailSent = false,
            Items = invoiceItems
        };

        _context.SalesInvoices.Add(invoice);
        _context.SaveChanges();

        return Ok(new
        {
            message = "Sales invoice created successfully.",
            invoiceId = invoice.Id,
            customerName = invoice.CustomerName,
            finalAmount = invoice.FinalAmount
        });
    }

    [HttpGet("report/daily")]
    public IActionResult GetDailyReport()
    {
        var today = DateTime.UtcNow.Date;

        var invoices = _context.SalesInvoices
            .Where(i => i.InvoiceDate.Date == today)
            .ToList();

        return Ok(new
        {
            reportType = "Daily Report",
            date = today.ToString("yyyy-MM-dd"),
            totalInvoices = invoices.Count,
            totalSales = invoices.Sum(i => i.TotalAmount),
            totalDiscount = invoices.Sum(i => i.DiscountAmount),
            finalIncome = invoices.Sum(i => i.FinalAmount),
            paidInvoices = invoices.Count(i => i.PaymentStatus == "Paid"),
            pendingInvoices = invoices.Count(i => i.PaymentStatus == "Pending"),
            creditInvoices = invoices.Count(i => i.PaymentStatus == "Credit")
        });
    }

    [HttpGet("report/monthly")]
    public IActionResult GetMonthlyReport()
    {
        var now = DateTime.UtcNow;

        var invoices = _context.SalesInvoices
            .Where(i => i.InvoiceDate.Month == now.Month && i.InvoiceDate.Year == now.Year)
            .ToList();

        return Ok(new
        {
            reportType = "Monthly Report",
            month = now.ToString("MMMM yyyy"),
            totalInvoices = invoices.Count,
            totalSales = invoices.Sum(i => i.TotalAmount),
            totalDiscount = invoices.Sum(i => i.DiscountAmount),
            finalIncome = invoices.Sum(i => i.FinalAmount),
            paidInvoices = invoices.Count(i => i.PaymentStatus == "Paid"),
            pendingInvoices = invoices.Count(i => i.PaymentStatus == "Pending"),
            creditInvoices = invoices.Count(i => i.PaymentStatus == "Credit")
        });
    }

    [HttpGet("report/yearly")]
    public IActionResult GetYearlyReport()
    {
        var now = DateTime.UtcNow;

        var invoices = _context.SalesInvoices
            .Where(i => i.InvoiceDate.Year == now.Year)
            .ToList();

        return Ok(new
        {
            reportType = "Yearly Report",
            year = now.Year,
            totalInvoices = invoices.Count,
            totalSales = invoices.Sum(i => i.TotalAmount),
            totalDiscount = invoices.Sum(i => i.DiscountAmount),
            finalIncome = invoices.Sum(i => i.FinalAmount),
            paidInvoices = invoices.Count(i => i.PaymentStatus == "Paid"),
            pendingInvoices = invoices.Count(i => i.PaymentStatus == "Pending"),
            creditInvoices = invoices.Count(i => i.PaymentStatus == "Credit")
        });
    }

    [HttpPost("{id}/send-email")]
    public IActionResult SendInvoiceEmail(int id)
    {
        var invoice = _context.SalesInvoices
            .Include(i => i.Items)
            .FirstOrDefault(i => i.Id == id);

        if (invoice == null)
            return NotFound(new { message = "Invoice not found." });

        if (string.IsNullOrWhiteSpace(invoice.CustomerEmail))
            return BadRequest(new { message = "Customer email not found." });

        var pdfBytes = _invoicePdfService.GenerateInvoicePdf(
            invoice.Id,
            invoice.CustomerName,
            invoice.CustomerEmail,
            invoice.FinalAmount
        );

        _emailService.SendInvoiceEmail(
            invoice.CustomerEmail,
            invoice.CustomerName,
            invoice.Id,
            invoice.FinalAmount,
            pdfBytes
        );

        invoice.EmailSent = true;
        _context.SaveChanges();

        return Ok(new { message = "Invoice email with PDF sent successfully." });
    }
    [HttpPost("send-credit-reminders")]
    public IActionResult SendCreditReminders()
    {
        var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

        var overdueInvoices = _context.SalesInvoices
            .Where(i =>
                (i.PaymentStatus == "Credit" || i.PaymentStatus == "Pending") &&
                i.InvoiceDate <= oneMonthAgo &&
                !string.IsNullOrWhiteSpace(i.CustomerEmail)
            )
            .ToList();

        if (overdueInvoices.Count == 0)
        {
            return Ok(new
            {
                message = "No overdue credit or pending invoices found.",
                remindersSent = 0
            });
        }

        foreach (var invoice in overdueInvoices)
        {
            _emailService.SendCreditReminderEmail(
                invoice.CustomerEmail,
                invoice.CustomerName,
                invoice.Id,
                invoice.FinalAmount,
                invoice.InvoiceDate
            );
        }

        return Ok(new
        {
            message = "Credit reminder emails sent successfully.",
            remindersSent = overdueInvoices.Count
        });
    }
    
    [HttpPost("{id}/make-overdue")]
    public IActionResult MakeInvoiceOverdue(int id)
    {
        var invoice = _context.SalesInvoices.FirstOrDefault(i => i.Id == id);

        if (invoice == null)
            return NotFound(new { message = "Invoice not found." });

        invoice.InvoiceDate = DateTime.UtcNow.AddMonths(-2);

        _context.SaveChanges();

        return Ok(new
        {
            message = "Invoice marked as overdue successfully."
        });
    }
}


public class SalesInvoiceRequest
{
    public string CustomerName { get; set; } = "";
    public string CustomerPhone { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public string VehicleNumber { get; set; } = "";
    public string PaymentStatus { get; set; } = "Paid";
    public List<SalesInvoiceItemRequest> Items { get; set; } = new();
}

public class SalesInvoiceItemRequest
{
    public int PartId { get; set; }
    public int Quantity { get; set; }
}