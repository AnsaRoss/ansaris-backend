using ErpApp.Domain;

namespace ErpApp.Application.Dtos.Invoice
{
    public class CustomerStatementDto
    {
        public int CustomerId { get; set; }
        public InvoiceType Type { get; set; }
        public DateTime AsOfDate { get; set; }
        public decimal TotalBilled { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public List<CustomerStatementInvoiceDto> Invoices { get; set; } = new();
    }
}
