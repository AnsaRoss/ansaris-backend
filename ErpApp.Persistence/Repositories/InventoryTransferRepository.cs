using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Persistence.Repositories
{
    public class InventoryTransferRepository : IInventoryTransferRepository
    {
        private readonly AppDbContext _context;

        public InventoryTransferRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(InventoryTransfer transfer)
        {
            await _context.InventoryTransfers.AddAsync(transfer);
        }

        public async Task<List<InventoryTransfer>> GetAllAsync()
        {
            return await _context.InventoryTransfers
                .Include(t => t.Items)
                .OrderByDescending(t => t.TransferDate)
                .ThenByDescending(t => t.Id)
                .ToListAsync();
        }

        public async Task<int> GetNextSequenceNumberAsync(DateTime date)
        {
            var currentMax = await _context.InventoryTransfers
                .Where(t => t.TransferDate.Year == date.Year)
                .Select(t => (int?)t.SequenceNumber)
                .MaxAsync() ?? 0;

            return currentMax + 1;
        }
    }
}
