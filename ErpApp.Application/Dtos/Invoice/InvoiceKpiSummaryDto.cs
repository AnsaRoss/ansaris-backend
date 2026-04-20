namespace ErpApp.Application.Dtos.Invoice
{
    public class InvoiceKpiSummaryDto
    {
        public int TotalInvoices { get; set; }
        public int PendingInvoices { get; set; }
        public int PaidInvoices { get; set; }
        public int CancelledInvoices { get; set; }
        public decimal TotalBilledAmount { get; set; }
        public decimal TotalCollectedOrPaidAmount { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal SalesOutstandingAmount { get; set; }
        public decimal PurchaseOutstandingAmount { get; set; }
    }
}
