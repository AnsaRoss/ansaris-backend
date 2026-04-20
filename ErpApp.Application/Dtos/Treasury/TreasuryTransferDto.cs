namespace ErpApp.Application.Dtos.Treasury
{
    public class TreasuryTransferDto
    {
        public int FromTreasuryAccountId { get; set; }
        public int ToTreasuryAccountId { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
