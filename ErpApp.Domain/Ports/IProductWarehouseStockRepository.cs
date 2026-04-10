using ErpApp.Domain.Entities;

namespace ErpApp.Domain.Ports
{
    public interface IProductWarehouseStockRepository
    {
        Task<ProductWarehouseStock?> GetAsync(int productId, int warehouseId);
        Task AddAsync(ProductWarehouseStock stock);
        Task<List<ProductWarehouseStock>> GetByWarehouseAsync(int warehouseId);
    }
}
