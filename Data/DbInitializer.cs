using Microsoft.EntityFrameworkCore;
using VLCTraders.Api.Models.Domain;

namespace VLCTraders.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Vendor> Vendors => Set<Vendor>();
        public DbSet<Material> Materials => Set<Material>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<Stock> Stock => Set<Stock>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<CrmVisit> CrmVisits => Set<CrmVisit>();
        public DbSet<Quotation> Quotations => Set<Quotation>();
        public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<UserTask> Tasks => Set<UserTask>();
        public DbSet<BankLedger> BankLedger => Set<BankLedger>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Mobile)
                .IsUnique();

            modelBuilder.Entity<Vendor>()
                .HasIndex(v => v.GstNumber)
                .IsUnique();

            modelBuilder.Entity<Material>()
                .HasIndex(m => m.MaterialName)
                .IsUnique();

            modelBuilder.Entity<Purchase>()
                .HasIndex(p => new { p.InvoiceNo, p.VendorId })
                .IsUnique();

            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.InvoiceNo)
                .IsUnique();

            modelBuilder.Entity<Quotation>()
                .HasIndex(q => q.QuotationNo)
                .IsUnique();

            modelBuilder.Entity<BankLedger>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Vendor)
                .WithMany(v => v.Purchases)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Material)
                .WithMany(m => m.Purchases)
                .HasForeignKey(p => p.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Material)
                .WithMany(m => m.Sales)
                .HasForeignKey(s => s.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CrmVisit>()
                .HasOne(c => c.Customer)
                .WithMany(c => c.CrmVisits)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quotation>()
                .HasOne(q => q.Customer)
                .WithMany(c => c.Quotations)
                .HasForeignKey(q => q.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuotationItem>()
                .HasOne(qi => qi.Quotation)
                .WithMany(q => q.QuotationItems)
                .HasForeignKey(qi => qi.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuotationItem>()
                .HasOne(qi => qi.Material)
                .WithMany(m => m.Purchases)
                .HasForeignKey(qi => qi.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
