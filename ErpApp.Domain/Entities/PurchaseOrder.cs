using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = "";
        public string Series { get; set; } = "PO";
        public int SequenceNumber { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Received, Cancelled
        public decimal SubtotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<PurchaseOrderDetail> Items { get; set; } = new List<PurchaseOrderDetail>();
    }

}
