using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.Dtos.PurchaseOrders
{
    public class PurchaseOrderCreateDto
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public int WarehouseId { get; set; }
        public string? Series { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }
}
