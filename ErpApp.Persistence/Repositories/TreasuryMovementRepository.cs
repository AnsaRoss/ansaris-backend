using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Persistence.Repositories
{
    public class TreasuryMovementRepository : ITreasuryMovementRepository
    {
        private readonly AppDbContext _context;

        public TreasuryMovementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TreasuryMovement movement)
        {
            await _context.TreasuryMovements.AddAsync(movement);
        }

        public async Task<List<TreasuryMovement>> GetAsync(int? accountId, DateTime? from, DateTime? to)
        {
            var query = _context.TreasuryMovements
                .Include(m => m.TreasuryAccount)
                .AsQueryable();

            if (accountId.HasValue)
                query = query.Where(m => m.TreasuryAccountId == accountId.Value);

            if (from.HasValue)
                query = query.Where(m => m.MovementDate >= from.Value);

            if (to.HasValue)
                query = query.Where(m => m.MovementDate <= to.Value);

            return await query
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<List<TreasuryMovement>> GetByReferenceAsync(string referenceType, string referenceNumber)
        {
            return await _context.TreasuryMovements
                .Include(m => m.TreasuryAccount)
                .Where(m => m.ReferenceType == referenceType && m.ReferenceNumber == referenceNumber)
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }
    }
}
