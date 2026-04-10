using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class ReverseInventoryMovementUseCase
    {
        private readonly IInventoryMovementRepository _movementRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductWarehouseStockRepository _warehouseStockRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReverseInventoryMovementUseCase(
            IInventoryMovementRepository movementRepository,
            IProductRepository productRepository,
            IProductWarehouseStockRepository warehouseStockRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _movementRepository = movementRepository;
            _productRepository = productRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdjustInventoryResultDto> ExecuteAsync(int movementId, ReverseInventoryMovementDto dto)
        {
            var movement = await _movementRepository.GetByIdAsync(movementId);
            if (movement == null)
                throw new Exception("Movimiento no encontrado.");

            if (movement.IsReversed)
                throw new Exception("El movimiento ya fue revertido.");

            if (movement.ReversalOfMovementId.HasValue)
                throw new Exception("No se puede revertir un movimiento de reversa.");

            var product = await _productRepository.GetByIdAsync(movement.ProductId);
            if (product == null)
                throw new Exception("Producto no encontrado para el movimiento.");

            var stockBefore = product.Stock;
            var reservedBefore = product.ReservedStock;

            var newStock = product.Stock - movement.Quantity;
            var newReserved = product.ReservedStock - movement.ReservedQuantityDelta;

            if (newStock < 0)
                throw new Exception("La reversa dejaría stock negativo.");

            if (newReserved < 0)
                throw new Exception("La reversa dejaría stock reservado negativo.");

            product.Stock = newStock;
            product.ReservedStock = newReserved;

            if (movement.WarehouseId.HasValue)
            {
                var whStock = await _warehouseStockRepository.GetAsync(product.Id, movement.WarehouseId.Value)
                    ?? throw new Exception("Stock por almacén no encontrado para revertir.");

                var whNewOnHand = whStock.OnHand - movement.Quantity;
                var whNewReserved = whStock.Reserved - movement.ReservedQuantityDelta;

                if (whNewOnHand < 0 || whNewReserved < 0 || whNewOnHand < whNewReserved)
                    throw new Exception("La reversa dejaría stock inconsistente en almacén.");

                whStock.OnHand = whNewOnHand;
                whStock.Reserved = whNewReserved;
            }

            var reverseMovement = new InventoryMovement
            {
                ProductId = product.Id,
                WarehouseId = movement.WarehouseId,
                MovementDate = DateTime.UtcNow,
                MovementType = movement.MovementType,
                Quantity = -movement.Quantity,
                StockBefore = stockBefore,
                StockAfter = product.Stock,
                ReservedQuantityDelta = -movement.ReservedQuantityDelta,
                ReservedBefore = reservedBefore,
                ReservedAfter = product.ReservedStock,
                UnitCost = movement.UnitCost,
                ReferenceType = $"Reversal-{movement.ReferenceType}",
                ReferenceNumber = movement.ReferenceNumber,
                Notes = dto.Reason,
                ReversalOfMovementId = movement.Id
            };

            movement.IsReversed = true;

            await _movementRepository.AddAsync(reverseMovement);
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Action = "ReverseInventoryMovement",
                EntityName = nameof(InventoryMovement),
                EntityId = movement.Id.ToString(),
                PerformedBy = string.IsNullOrWhiteSpace(dto.PerformedBy) ? "system" : dto.PerformedBy,
                Details = $"Reversed movement {movement.Id}. Reason: {dto.Reason}"
            });

            await _unitOfWork.CommitAsync();

            return new AdjustInventoryResultDto
            {
                MovementId = reverseMovement.Id,
                ProductId = product.Id,
                QuantityDelta = reverseMovement.Quantity,
                StockBefore = stockBefore,
                StockAfter = product.Stock,
                MovementDate = reverseMovement.MovementDate
            };
        }
    }
}
