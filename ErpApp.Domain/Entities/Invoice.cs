using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public string Series { get; set; } = "A";
        public int SequenceNumber { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceType Type { get; set; } // Sale or Purchase
        public int CustomerId { get; set; } // o SupplierId según tipo
        public int? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public InvoiceStatus Status { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();
    }
}
