using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } // Transferencia, Efectivo, etc.
    }

}
