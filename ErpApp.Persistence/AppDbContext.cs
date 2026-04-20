
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
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<ProductWarehouseStock> ProductWarehouseStocks { get; set; }
        public DbSet<InventoryTransfer> InventoryTransfers { get; set; }
        public DbSet<InventoryTransferItem> InventoryTransferItems { get; set; }
        public DbSet<TreasuryAccount> TreasuryAccounts { get; set; }
        public DbSet<TreasuryMovement> TreasuryMovements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.RowVersion)
                .IsRowVersion();
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

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Warehouse)
                .WithMany()
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasOne(i => i.Warehouse)
                .WithMany()
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Items)
                .WithOne()
                .HasForeignKey(i => i.InvoiceId);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<InventoryMovement>()
                .Property(i => i.UnitCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId);

            modelBuilder.Entity<InventoryMovement>()
                .HasOne(i => i.Warehouse)
                .WithMany()
                .HasForeignKey(i => i.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Warehouse>()
                .HasIndex(w => w.Code)
                .IsUnique();

            modelBuilder.Entity<ProductWarehouseStock>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<ProductWarehouseStock>()
                .HasOne(s => s.Warehouse)
                .WithMany()
                .HasForeignKey(s => s.WarehouseId);

            modelBuilder.Entity<ProductWarehouseStock>()
                .HasIndex(s => new { s.ProductId, s.WarehouseId })
                .IsUnique();

            modelBuilder.Entity<ProductWarehouseStock>()
                .Property(s => s.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<InventoryTransfer>()
                .HasOne(t => t.SourceWarehouse)
                .WithMany()
                .HasForeignKey(t => t.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransfer>()
                .HasOne(t => t.DestinationWarehouse)
                .WithMany()
                .HasForeignKey(t => t.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransferItem>()
                .Property(i => i.UnitCost)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<InventoryTransferItem>()
                .HasOne(i => i.InventoryTransfer)
                .WithMany(t => t.Items)
                .HasForeignKey(i => i.InventoryTransferId);

            modelBuilder.Entity<InventoryTransferItem>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId);

            modelBuilder.Entity<TreasuryAccount>()
                .HasIndex(a => a.Code)
                .IsUnique();

            modelBuilder.Entity<TreasuryAccount>()
                .Property(a => a.Balance)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TreasuryMovement>()
                .Property(m => m.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TreasuryMovement>()
                .Property(m => m.BalanceBefore)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TreasuryMovement>()
                .Property(m => m.BalanceAfter)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<TreasuryMovement>()
                .HasOne(m => m.TreasuryAccount)
                .WithMany()
                .HasForeignKey(m => m.TreasuryAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TreasuryMovement>()
                .HasOne(m => m.CounterpartyTreasuryAccount)
                .WithMany()
                .HasForeignKey(m => m.CounterpartyTreasuryAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuditLog>()
                .Property(a => a.Details)
                .HasMaxLength(2000);


        }
    }
}
