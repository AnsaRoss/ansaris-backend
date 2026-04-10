using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.Dtos.PurchaseOrders
{
    public class PurchaseOrderReadDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int WarehouseId { get; set; }
        public string Status { get; set; } = "Pending";
        public List<PurchaseOrderItemReadDto> Items { get; set; } = new();
    }

    public class PurchaseOrderItemReadDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
