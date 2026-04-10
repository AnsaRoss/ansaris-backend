using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.Dtos.SaleOrders
{
    public class SalesOrderCreateDto
    {
        public int? CustomerId { get; set; } // opcional por ahora
        public DateTime OrderDate { get; set; }
        public List<SalesOrderItemDto> Items { get; set; }
    }
    public class SalesOrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
