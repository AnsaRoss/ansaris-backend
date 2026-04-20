using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.Dtos.Invoice
{
    public class RegisterPaymentDto
    {
        public int TreasuryAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
    }
}
