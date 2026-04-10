using ErpApp.Domain;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ErpApp.Application.Dtos.Inventory;

namespace ErpApp.Application.UseCases.Inventory
{
    public class ReserveStockUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryMovementRepository _movementRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductWarehouseStockRepository _warehouseStockRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReserveStockUseCase(
            IProductRepository productRepository,
            IInventoryMovementRepository movementRepository,
            IWarehouseRepository warehouseRepository,
            IProductWarehouseStockRepository warehouseStockRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _movementRepository = movementRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AdjustInventoryResultDto> ExecuteAsync(ReserveStockDto dto)
        {
            if (dto.Quantity <= 0)
                throw new Exception("La cantidad a reservar debe ser mayor que cero.");

            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new Exception($"Producto con ID {dto.ProductId} no encontrado.");

            var available = product.Stock - product.ReservedStock;
            if (available < dto.Quantity)
                throw new Exception($"Stock disponible insuficiente. Disponible: {available}.");

            var stockBefore = product.Stock;
            var reservedBefore = product.ReservedStock;

            product.ReservedStock += dto.Quantity;

            if (dto.WarehouseId.HasValue)
            {
                var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId.Value)
                    ?? throw new Exception("Almacén no encontrado.");

                var whStock = await _warehouseStockRepository.GetAsync(product.Id, warehouse.Id)
                    ?? throw new Exception("No hay stock del producto en el almacén indicado.");

                var whAvailable = whStock.OnHand - whStock.Reserved;
                if (whAvailable < dto.Quantity)
                    throw new Exception($"Stock disponible insuficiente en almacén {warehouse.Code}. Disponible: {whAvailable}.");

                whStock.Reserved += dto.Quantity;
            }

            var movement = new InventoryMovement
            {
                ProductId = product.Id,
                WarehouseId = dto.WarehouseId,
                MovementDate = DateTime.UtcNow,
                MovementType = InventoryMovementType.Reservation,
                Quantity = 0,
                StockBefore = stockBefore,
                StockAfter = product.Stock,
                ReservedQuantityDelta = dto.Quantity,
                ReservedBefore = reservedBefore,
                ReservedAfter = product.ReservedStock,
                UnitCost = product.Price,
                ReferenceType = dto.ReferenceType,
                ReferenceNumber = dto.ReferenceNumber,
                Notes = dto.Notes
            };

            await _movementRepository.AddAsync(movement);
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Action = "ReserveStock",
                EntityName = nameof(Product),
                EntityId = product.Id.ToString(),
                PerformedBy = string.IsNullOrWhiteSpace(dto.PerformedBy) ? "system" : dto.PerformedBy,
                Details = $"Reserved {dto.Quantity}. Ref {dto.ReferenceType}-{dto.ReferenceNumber}"
            });

            await _unitOfWork.CommitAsync();

            return new AdjustInventoryResultDto
            {
                MovementId = movement.Id,
                ProductId = product.Id,
                QuantityDelta = 0,
                StockBefore = stockBefore,
                StockAfter = product.Stock,
                MovementDate = movement.MovementDate
            };
        }
    }
}
