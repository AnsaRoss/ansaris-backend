using ErpApp.Domain;

namespace ErpApp.Domain.Entities
{
    public class TreasuryMovement
    {
        public int Id { get; set; }
        public DateTime MovementDate { get; set; } = DateTime.UtcNow;
        public int TreasuryAccountId { get; set; }
        public TreasuryAccount TreasuryAccount { get; set; } = null!;
        public int? CounterpartyTreasuryAccountId { get; set; }
        public TreasuryAccount? CounterpartyTreasuryAccount { get; set; }
        public TreasuryMovementType Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public string ReferenceType { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
