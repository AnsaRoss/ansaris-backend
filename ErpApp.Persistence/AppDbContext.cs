
using ErpApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERPApp.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<PurchaseOrderDetail>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.Items)
                .WithOne(d => d.PurchaseOrder)
                .HasForeignKey(d => d.PurchaseOrderId);

            modelBuilder.Entity<PurchaseOrderDetail>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.SubtotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.TaxRate)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.TaxAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PurchaseOrder>()
                .Property(p => p.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.SubtotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TaxRate)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TaxAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .Property(i => i.PaidAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Items)
                .WithOne()
                .HasForeignKey(i => i.InvoiceId);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");


        }
    }
}
