using ErpApp.Domain;

namespace ErpApp.Application.Dtos.Treasury
{
    public class TreasuryAccountDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public TreasuryAccountType Type { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
    }
}
