using ErpApp.Domain;

namespace ErpApp.Domain.Entities
{
    public class TreasuryAccount
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public TreasuryAccountType Type { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
