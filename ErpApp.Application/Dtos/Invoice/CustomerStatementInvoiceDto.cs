namespace ErpApp.Application.Dtos.Invoice
{
    public class CustomerStatementInvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public int DaysPastDue { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
