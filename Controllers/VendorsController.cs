using Microsoft.AspNetCore.Mvc;
using VehicleParts.API.Data;
using VehicleParts.API.Models;
using VehicleParts.API.ViewModels;

namespace VehicleParts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public VendorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetVendors()
    {
        return Ok(_context.Vendors.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetVendorById(int id)
    {
        var vendor = _context.Vendors.Find(id);

        if (vendor == null)
            return NotFound(new { message = "Vendor not found" });

        return Ok(vendor);
    }

    [HttpPost]
    public IActionResult AddVendor(VendorViewModel model)
    {
        var vendor = new Vendor
        {
            VendorName = model.VendorName,
            ContactPerson = model.ContactPerson,
            PhoneNumber = model.PhoneNumber,
            Address = model.Address
        };

        _context.Vendors.Add(vendor);
        _context.SaveChanges();

        return Ok(vendor);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateVendor(int id, VendorViewModel model)
    {
        var vendor = _context.Vendors.Find(id);

        if (vendor == null)
            return NotFound(new { message = "Vendor not found" });

        vendor.VendorName = model.VendorName;
        vendor.ContactPerson = model.ContactPerson;
        vendor.PhoneNumber = model.PhoneNumber;
        vendor.Address = model.Address;

        _context.SaveChanges();

        return Ok(new { message = "Vendor updated successfully", data = vendor });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteVendor(int id)
    {
        var vendor = _context.Vendors.Find(id);

        if (vendor == null)
            return NotFound(new { message = "Vendor not found" });

        _context.Vendors.Remove(vendor);
        _context.SaveChanges();

        return Ok(new { message = "Vendor deleted successfully" });
    }
}