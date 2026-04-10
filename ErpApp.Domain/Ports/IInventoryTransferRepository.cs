using ErpApp.Domain.Entities;

namespace ErpApp.Domain.Ports
{
    public interface IInventoryTransferRepository
    {
        Task AddAsync(InventoryTransfer transfer);
        Task<List<InventoryTransfer>> GetAllAsync();
        Task<int> GetNextSequenceNumberAsync(DateTime date);
    }
}
