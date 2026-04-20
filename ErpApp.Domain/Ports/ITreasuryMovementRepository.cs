using ErpApp.Domain.Entities;

namespace ErpApp.Domain.Ports
{
    public interface ITreasuryMovementRepository
    {
        Task AddAsync(TreasuryMovement movement);
        Task<List<TreasuryMovement>> GetAsync(int? accountId, DateTime? from, DateTime? to);
        Task<List<TreasuryMovement>> GetByReferenceAsync(string referenceType, string referenceNumber);
    }
}
