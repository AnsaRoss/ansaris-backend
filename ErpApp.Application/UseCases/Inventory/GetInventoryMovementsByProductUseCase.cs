using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class GetInventoryMovementsByProductUseCase
    {
        private readonly IInventoryMovementRepository _movementRepository;

        public GetInventoryMovementsByProductUseCase(IInventoryMovementRepository movementRepository)
        {
            _movementRepository = movementRepository;
        }

        public async Task<List<InventoryMovement>> ExecuteAsync(int productId, DateTime? from, DateTime? to)
        {
            return await _movementRepository.GetByProductAsync(productId, from, to);
        }
    }
}
