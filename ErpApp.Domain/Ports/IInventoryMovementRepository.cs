using ErpApp.Domain.Entities;
using ErpApp.Domain;

namespace ErpApp.Domain.Ports
{
    public interface IInventoryMovementRepository
    {
        Task AddAsync(InventoryMovement movement);
        Task<InventoryMovement?> GetByIdAsync(int id);
        Task<List<InventoryMovement>> GetByProductAsync(int productId, DateTime? from, DateTime? to);
        Task<(List<InventoryMovement> Items, int TotalCount)> SearchAsync(
            int? productId,
            DateTime? from,
            DateTime? to,
            InventoryMovementType? movementType,
            string? referenceType,
            string? referenceNumber,
            int pageNumber,
            int pageSize);
    }
}
