using ErpApp.Domain.Entities;
using ErpApp.Domain;
using ErpApp.Domain.Ports;
using ERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Persistence.Repositories
{
    public class InventoryMovementRepository : IInventoryMovementRepository
    {
        private readonly AppDbContext _context;

        public InventoryMovementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InventoryMovement movement)
        {
            await _context.InventoryMovements.AddAsync(movement);
        }

        public async Task<InventoryMovement?> GetByIdAsync(int id)
        {
            return await _context.InventoryMovements.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<InventoryMovement>> GetByProductAsync(int productId, DateTime? from, DateTime? to)
        {
            var query = _context.InventoryMovements
                .Where(m => m.ProductId == productId);

            if (from.HasValue)
            {
                query = query.Where(m => m.MovementDate >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(m => m.MovementDate <= to.Value);
            }

            return await query
                .OrderBy(m => m.MovementDate)
                .ThenBy(m => m.Id)
                .ToListAsync();
        }

        public async Task<(List<InventoryMovement> Items, int TotalCount)> SearchAsync(
            int? productId,
            DateTime? from,
            DateTime? to,
            InventoryMovementType? movementType,
            string? referenceType,
            string? referenceNumber,
            int pageNumber,
            int pageSize)
        {
            var query = _context.InventoryMovements.AsQueryable();

            if (productId.HasValue)
                query = query.Where(m => m.ProductId == productId.Value);

            if (from.HasValue)
                query = query.Where(m => m.MovementDate >= from.Value);

            if (to.HasValue)
                query = query.Where(m => m.MovementDate <= to.Value);

            if (movementType.HasValue)
                query = query.Where(m => m.MovementType == movementType.Value);

            if (!string.IsNullOrWhiteSpace(referenceType))
                query = query.Where(m => m.ReferenceType == referenceType);

            if (!string.IsNullOrWhiteSpace(referenceNumber))
                query = query.Where(m => m.ReferenceNumber.Contains(referenceNumber));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.MovementDate)
                .ThenByDescending(m => m.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
