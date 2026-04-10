using ErpApp.Domain.Entities;

namespace ErpApp.Domain.Ports
{
    public interface IWarehouseRepository
    {
        Task AddAsync(Warehouse warehouse);
        Task<Warehouse?> GetByIdAsync(int id);
        Task<Warehouse?> GetByCodeAsync(string code);
        Task<List<Warehouse>> GetAllAsync();
    }
}
