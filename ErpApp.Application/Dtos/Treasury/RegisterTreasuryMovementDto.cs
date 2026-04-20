using ErpApp.Domain;

namespace ErpApp.Application.Dtos.Treasury
{
    public class RegisterTreasuryMovementDto
    {
        public int TreasuryAccountId { get; set; }
        public TreasuryMovementType Type { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceType { get; set; } = "Manual";
        public string ReferenceNumber { get; set; } = "MANUAL";
        public string? Notes { get; set; }
    }
}
