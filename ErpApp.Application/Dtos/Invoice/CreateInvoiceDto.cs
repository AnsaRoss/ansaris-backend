using ErpApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.Dtos.Invoice
{
    public class CreateInvoiceDto
    {
        public InvoiceType Type { get; set; }
        public int CustomerId { get; set; }
        public int? WarehouseId { get; set; }
        public string? Series { get; set; }
        public decimal TaxRate { get; set; }
        public decimal DiscountAmount { get; set; }

        public List<CreateInvoiceItemDto> Items { get; set; }
    }
}
