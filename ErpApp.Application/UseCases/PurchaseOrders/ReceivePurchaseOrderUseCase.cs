using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ErpApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.PurchaseOrders
{
    public class ReceivePurchaseOrderUseCase
    {
        private readonly IPurchaseOrderRepository _purchaseRepo;
        private readonly IProductRepository _productRepo;
        private readonly IProductWarehouseStockRepository _warehouseStockRepository;
        private readonly IInventoryMovementRepository _movementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReceivePurchaseOrderUseCase(
            IPurchaseOrderRepository purchaseRepo,
            IProductRepository productRepo,
            IProductWarehouseStockRepository warehouseStockRepository,
            IInventoryMovementRepository movementRepository,
            IUnitOfWork unitOfWork)
        {
            _purchaseRepo = purchaseRepo;
            _productRepo = productRepo;
            _warehouseStockRepository = warehouseStockRepository;
            _movementRepository = movementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PurchaseOrder> ExecuteAsync(string orderNumber)
        {
            var order = await _purchaseRepo.GetByIdAsync(orderNumber);
            if (order == null) throw new Exception("Orden no encontrada.");
            if (order.Status == "Received") throw new Exception("La orden ya fue recibida.");
            if (order.Status == "Cancelled") throw new Exception("No se puede recibir una orden anulada.");

            // Aumentar stock
            foreach (var item in order.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product == null) continue;

                var stockBefore = product.Stock;
                product.Stock += item.Quantity;

                var warehouseStock = await _warehouseStockRepository.GetAsync(product.Id, order.WarehouseId);
                if (warehouseStock == null)
                {
                    warehouseStock = new ProductWarehouseStock
                    {
                        ProductId = product.Id,
                        WarehouseId = order.WarehouseId,
                        OnHand = 0,
                        Reserved = 0
                    };

                    await _warehouseStockRepository.AddAsync(warehouseStock);
                }

                var warehouseStockBefore = warehouseStock.OnHand;
                warehouseStock.OnHand += item.Quantity;

                var movement = new InventoryMovement
                {
                    ProductId = product.Id,
                    WarehouseId = order.WarehouseId,
                    MovementDate = DateTime.UtcNow,
                    MovementType = InventoryMovementType.Inbound,
                    Quantity = item.Quantity,
                    StockBefore = warehouseStockBefore,
                    StockAfter = warehouseStock.OnHand,
                    ReservedQuantityDelta = 0,
                    ReservedBefore = warehouseStock.Reserved,
                    ReservedAfter = warehouseStock.Reserved,
                    UnitCost = item.UnitPrice,
                    ReferenceType = "PurchaseOrder",
                    ReferenceNumber = order.OrderNumber,
                    Notes = "Recepción de orden de compra"
                };

                await _movementRepository.AddAsync(movement);
            }

            order.Status = "Received";
            await _unitOfWork.CommitAsync();
            return order;
        }
    }

}
