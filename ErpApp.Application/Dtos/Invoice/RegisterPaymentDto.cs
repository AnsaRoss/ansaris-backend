using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.Dtos.Invoice
{
    public class RegisterPaymentDto
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
