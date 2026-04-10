using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Persistence.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _context;

        public WarehouseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Warehouse warehouse)
        {
            await _context.Warehouses.AddAsync(warehouse);
        }

        public async Task<Warehouse?> GetByIdAsync(int id)
        {
            return await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Warehouse?> GetByCodeAsync(string code)
        {
            return await _context.Warehouses.FirstOrDefaultAsync(w => w.Code == code);
        }

        public async Task<List<Warehouse>> GetAllAsync()
        {
            return await _context.Warehouses
                .OrderBy(w => w.Code)
                .ToListAsync();
        }
    }
}
