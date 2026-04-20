using ErpApp.Domain;

namespace ErpApp.Application.Dtos.Treasury
{
    public class TreasuryMovementDto
    {
        public int Id { get; set; }
        public DateTime MovementDate { get; set; }
        public int TreasuryAccountId { get; set; }
        public string TreasuryAccountCode { get; set; } = string.Empty;
        public TreasuryMovementType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string ReferenceType { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
