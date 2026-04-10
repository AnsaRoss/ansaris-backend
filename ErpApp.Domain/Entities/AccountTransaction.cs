using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Entities
{
    public class AccountTransaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Guid DebitAccountId { get; set; }
        public Guid CreditAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int? InvoiceId { get; set; } // Opcional, enlaza con una factura
    }

}
