using ErpApp.Domain.Entities;

namespace ErpApp.Domain.Ports
{
    public interface ITreasuryAccountRepository
    {
        Task AddAsync(TreasuryAccount account);
        Task<TreasuryAccount?> GetByIdAsync(int id);
        Task<TreasuryAccount?> GetByCodeAsync(string code);
        Task<List<TreasuryAccount>> GetAllAsync();
    }
}
