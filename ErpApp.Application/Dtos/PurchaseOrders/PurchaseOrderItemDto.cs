using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.Dtos.PurchaseOrders
{
    public class PurchaseOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
