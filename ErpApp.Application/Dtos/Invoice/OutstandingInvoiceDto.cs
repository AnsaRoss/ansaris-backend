namespace ErpApp.Application.Dtos.Invoice
{
    public class OutstandingInvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int? WarehouseId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public int DaysOpen { get; set; }
        public int DaysPastDue { get; set; }
    }
}
