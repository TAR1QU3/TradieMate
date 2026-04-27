using Microsoft.EntityFrameworkCore;
using TradieMateWeb.API.Models;

namespace TradieMateWeb.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<BusinessSettings> BusinessSettings { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Job ka TotalAmount calculated hai - database mein save nahi hoga
        modelBuilder.Entity<Job>()
            .Ignore(j => j.TotalAmount);

        // BusinessSettings ka sirf ek record hoga
        modelBuilder.Entity<BusinessSettings>()
            .HasData(new BusinessSettings
            {
                Id = 1,
                BusinessName = "",
                OwnerName = "",
                ABN = "",
                Phone = "",
                Email = "",
                Address = "",
                BankName = "",
                BSB = "",
                AccountNumber = "",
                PayID = "",
                PaymentTerms = "Net 14",
                InvoiceNotes = "Thank you for your business!"
            });
    }
}