using ErpApp.Domain;

namespace ErpApp.Application.Dtos.Treasury
{
    public class TreasuryAccountCreateDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public TreasuryAccountType Type { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal OpeningBalance { get; set; }
    }
}
