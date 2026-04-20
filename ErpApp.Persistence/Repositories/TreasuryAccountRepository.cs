using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Persistence.Repositories
{
    public class TreasuryAccountRepository : ITreasuryAccountRepository
    {
        private readonly AppDbContext _context;

        public TreasuryAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TreasuryAccount account)
        {
            await _context.TreasuryAccounts.AddAsync(account);
        }

        public async Task<TreasuryAccount?> GetByIdAsync(int id)
        {
            return await _context.TreasuryAccounts.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<TreasuryAccount?> GetByCodeAsync(string code)
        {
            return await _context.TreasuryAccounts.FirstOrDefaultAsync(a => a.Code == code);
        }

        public async Task<List<TreasuryAccount>> GetAllAsync()
        {
            return await _context.TreasuryAccounts
                .OrderBy(a => a.Code)
                .ToListAsync();
        }
    }
}
