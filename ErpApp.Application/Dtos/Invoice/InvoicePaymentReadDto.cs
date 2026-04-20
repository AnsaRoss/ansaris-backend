namespace ErpApp.Application.Dtos.Invoice
{
    public class InvoicePaymentReadDto
    {
        public int TreasuryMovementId { get; set; }
        public int TreasuryAccountId { get; set; }
        public string TreasuryAccountCode { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
    }
}
