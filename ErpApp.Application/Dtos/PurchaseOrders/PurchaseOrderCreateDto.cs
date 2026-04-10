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
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }
}
