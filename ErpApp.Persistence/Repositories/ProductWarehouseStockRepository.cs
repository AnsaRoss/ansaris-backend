using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ErpApp.Persistence.Repositories
{
    public class ProductWarehouseStockRepository : IProductWarehouseStockRepository
    {
        private readonly AppDbContext _context;

        public ProductWarehouseStockRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductWarehouseStock?> GetAsync(int productId, int warehouseId)
        {
            return await _context.ProductWarehouseStocks
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);
        }

        public async Task AddAsync(ProductWarehouseStock stock)
        {
            await _context.ProductWarehouseStocks.AddAsync(stock);
        }

        public async Task<List<ProductWarehouseStock>> GetByWarehouseAsync(int warehouseId)
        {
            return await _context.ProductWarehouseStocks
                .Include(s => s.Product)
                .Include(s => s.Warehouse)
                .Where(s => s.WarehouseId == warehouseId)
                .OrderBy(s => s.Product.Name)
                .ToListAsync();
        }
    }
}
