using Microsoft.EntityFrameworkCore;
using VehicleParts.API.Models;

namespace VehicleParts.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Part> Parts { get; set; }
    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<SalesInvoice> SalesInvoices { get; set; }
    
    public DbSet<SalesInvoiceItem> SalesInvoiceItems { get; set; }
    
    public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
    
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; }
    
    public DbSet<Appointment> Appointments { get; set; }
    
    public DbSet<UnavailablePartRequest> UnavailablePartRequests { get; set; }
    
    public DbSet<ServiceReview> ServiceReviews { get; set; }
}