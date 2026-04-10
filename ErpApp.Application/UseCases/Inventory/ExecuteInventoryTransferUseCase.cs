using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class ExecuteInventoryTransferUseCase
    {
        private readonly IInventoryTransferRepository _transferRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductWarehouseStockRepository _warehouseStockRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryMovementRepository _movementRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExecuteInventoryTransferUseCase(
            IInventoryTransferRepository transferRepository,
            IWarehouseRepository warehouseRepository,
            IProductWarehouseStockRepository warehouseStockRepository,
            IProductRepository productRepository,
            IInventoryMovementRepository movementRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
        {
            _transferRepository = transferRepository;
            _warehouseRepository = warehouseRepository;
            _warehouseStockRepository = warehouseStockRepository;
            _productRepository = productRepository;
            _movementRepository = movementRepository;
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InventoryTransferResultDto> ExecuteAsync(InventoryTransferCreateDto dto)
        {
            if (dto.SourceWarehouseId == dto.DestinationWarehouseId)
                throw new Exception("El almacén origen y destino no pueden ser iguales.");

            if (dto.Items == null || dto.Items.Count == 0)
                throw new Exception("La transferencia debe incluir al menos un producto.");

            var source = await _warehouseRepository.GetByIdAsync(dto.SourceWarehouseId)
                ?? throw new Exception("Almacén origen no encontrado.");
            var destination = await _warehouseRepository.GetByIdAsync(dto.DestinationWarehouseId)
                ?? throw new Exception("Almacén destino no encontrado.");

            if (!source.IsActive || !destination.IsActive)
                throw new Exception("Solo se permiten transferencias entre almacenes activos.");

            var transferDate = DateTime.UtcNow;
            var nextSequence = await _transferRepository.GetNextSequenceNumberAsync(transferDate);
            var transferNumber = $"TR-{transferDate:yyyyMMdd}-{nextSequence:D6}";

            var transfer = new InventoryTransfer
            {
                SequenceNumber = nextSequence,
                TransferNumber = transferNumber,
                TransferDate = transferDate,
                SourceWarehouseId = source.Id,
                DestinationWarehouseId = destination.Id,
                Status = InventoryTransferStatus.Completed,
                Notes = dto.Notes
            };

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception($"Cantidad inválida para producto {item.ProductId}.");

                var product = await _productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new Exception($"Producto con ID {item.ProductId} no encontrado.");

                var sourceStock = await _warehouseStockRepository.GetAsync(product.Id, source.Id);
                if (sourceStock == null)
                    throw new Exception($"No hay stock del producto {product.Name} en almacén origen.");

                var sourceAvailable = sourceStock.OnHand - sourceStock.Reserved;
                if (sourceAvailable < item.Quantity)
                    throw new Exception($"Stock insuficiente para producto {product.Name} en almacén origen. Disponible: {sourceAvailable}.");

                var destinationStock = await _warehouseStockRepository.GetAsync(product.Id, destination.Id);
                if (destinationStock == null)
                {
                    destinationStock = new ProductWarehouseStock
                    {
                        ProductId = product.Id,
                        WarehouseId = destination.Id,
                        OnHand = 0,
                        Reserved = 0
                    };
                    await _warehouseStockRepository.AddAsync(destinationStock);
                }

                var sourceBefore = sourceStock.OnHand;
                sourceStock.OnHand -= item.Quantity;

                var destinationBefore = destinationStock.OnHand;
                destinationStock.OnHand += item.Quantity;

                transfer.Items.Add(new InventoryTransferItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitCost = product.Price
                });

                await _movementRepository.AddAsync(new InventoryMovement
                {
                    ProductId = product.Id,
                    WarehouseId = source.Id,
                    MovementDate = transferDate,
                    MovementType = InventoryMovementType.Outbound,
                    Quantity = -item.Quantity,
                    StockBefore = sourceBefore,
                    StockAfter = sourceStock.OnHand,
                    ReservedBefore = sourceStock.Reserved,
                    ReservedAfter = sourceStock.Reserved,
                    UnitCost = product.Price,
                    ReferenceType = "TransferOut",
                    ReferenceNumber = transferNumber,
                    Notes = $"Transferencia a almacén {destination.Code}"
                });

                await _movementRepository.AddAsync(new InventoryMovement
                {
                    ProductId = product.Id,
                    WarehouseId = destination.Id,
                    MovementDate = transferDate,
                    MovementType = InventoryMovementType.Inbound,
                    Quantity = item.Quantity,
                    StockBefore = destinationBefore,
                    StockAfter = destinationStock.OnHand,
                    ReservedBefore = destinationStock.Reserved,
                    ReservedAfter = destinationStock.Reserved,
                    UnitCost = product.Price,
                    ReferenceType = "TransferIn",
                    ReferenceNumber = transferNumber,
                    Notes = $"Transferencia desde almacén {source.Code}"
                });
            }

            await _transferRepository.AddAsync(transfer);
            await _auditLogRepository.AddAsync(new AuditLog
            {
                Action = "ExecuteInventoryTransfer",
                EntityName = nameof(InventoryTransfer),
                EntityId = transfer.TransferNumber,
                PerformedBy = string.IsNullOrWhiteSpace(dto.PerformedBy) ? "system" : dto.PerformedBy,
                Details = $"From {source.Code} to {destination.Code}. Items: {transfer.Items.Count}"
            });

            await _unitOfWork.CommitAsync();

            return new InventoryTransferResultDto
            {
                TransferId = transfer.Id,
                TransferNumber = transfer.TransferNumber,
                TransferDate = transfer.TransferDate,
                SourceWarehouseId = transfer.SourceWarehouseId,
                DestinationWarehouseId = transfer.DestinationWarehouseId,
                ItemsCount = transfer.Items.Count
            };
        }
    }
}
