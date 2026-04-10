using ErpApp.Application.Dtos.Inventory;
using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class AdjustInventoryUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryMovementRepository _movementRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductWarehouseStockRepository _warehouseStockRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdjustInventoryUseCase(
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

        public async Task<AdjustInventoryResultDto> ExecuteAsync(AdjustInventoryDto dto)
        {
            if (dto.QuantityDelta == 0)
                throw new Exception("El ajuste debe ser distinto de cero.");

            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new Exception($"Producto con ID {dto.ProductId} no encontrado.");

            var stockBefore = product.Stock;
            var stockAfter = stockBefore + dto.QuantityDelta;
            var reservedBefore = product.ReservedStock;

            if (stockAfter < 0)
                throw new Exception("El ajuste dejaría stock negativo.");

            if (stockAfter < product.ReservedStock)
                throw new Exception("El ajuste dejaría stock menor al reservado.");

            product.Stock = stockAfter;

            ProductWarehouseStock? warehouseStock = null;
            if (dto.WarehouseId.HasValue)
            {
                var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId.Value)
                    ?? throw new Exception("Almacén no encontrado.");

                if (!warehouse.IsActive)
                    throw new Exception("No se puede ajustar inventario en almacén inactivo.");

                warehouseStock = await _warehouseStockRepository.GetAsync(product.Id, warehouse.Id);
                if (warehouseStock == null)
                {
                    warehouseStock = new ProductWarehouseStock
                    {
                        ProductId = product.Id,
                        WarehouseId = warehouse.Id,
                        OnHand = 0,
                        Reserved = 0
                    };

                    await _warehouseStockRepository.AddAsync(warehouseStock);
                }

                var whAfter = warehouseStock.OnHand + dto.QuantityDelta;
                if (whAfter < 0)
                    throw new Exception("El ajuste dejaría stock negativo en almacén.");

                if (whAfter < warehouseStock.Reserved)
                    throw new Exception("El ajuste dejaría stock de almacén menor al reservado.");

                warehouseStock.OnHand = whAfter;
            }

            var movement = new InventoryMovement
            {
                ProductId = product.Id,
                WarehouseId = dto.WarehouseId,
                MovementDate = DateTime.UtcNow,
                MovementType = InventoryMovementType.Adjustment,
                Quantity = dto.QuantityDelta,
                StockBefore = stockBefore,
                StockAfter = stockAfter,
                ReservedQuantityDelta = 0,
                ReservedBefore = reservedBefore,
                ReservedAfter = product.ReservedStock,
                UnitCost = dto.UnitCost,
                ReferenceType = "Adjustment",
                ReferenceNumber = "MANUAL",
                Notes = dto.Notes
            };

            await _movementRepository.AddAsync(movement);
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Action = "AdjustInventory",
                EntityName = nameof(Product),
                EntityId = product.Id.ToString(),
                PerformedBy = string.IsNullOrWhiteSpace(dto.PerformedBy) ? "system" : dto.PerformedBy,
                Details = $"Delta: {dto.QuantityDelta}. Notes: {dto.Notes}"
            });
            await _unitOfWork.CommitAsync();

            return new AdjustInventoryResultDto
            {
                MovementId = movement.Id,
                ProductId = product.Id,
                QuantityDelta = dto.QuantityDelta,
                StockBefore = stockBefore,
                StockAfter = stockAfter,
                MovementDate = movement.MovementDate
            };
        }
    }
}
