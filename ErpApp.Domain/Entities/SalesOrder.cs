using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Entities
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } // Pending, Paid, Cancelled, Delivered...
        public int? CustomerId { get; set; }
        public List<SalesOrderDetail> Items { get; set; }
        public decimal Total => Items?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;
    }

}
