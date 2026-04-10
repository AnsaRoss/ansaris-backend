using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class GetInventoryValuationUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryMovementRepository _movementRepository;

        public GetInventoryValuationUseCase(
            IProductRepository productRepository,
            IInventoryMovementRepository movementRepository)
        {
            _productRepository = productRepository;
            _movementRepository = movementRepository;
        }

        public async Task<InventoryValuationDto> ExecuteAsync(int productId, string method)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Producto no encontrado.");

            var movements = await _movementRepository.GetByProductAsync(productId, null, null);
            var normalizedMethod = string.IsNullOrWhiteSpace(method) ? "average" : method.Trim().ToLowerInvariant();

            return normalizedMethod switch
            {
                "fifo" => CalculateFifo(productId, movements),
                _ => CalculateAverage(productId, movements)
            };
        }

        private static InventoryValuationDto CalculateAverage(int productId, List<Domain.Entities.InventoryMovement> movements)
        {
            decimal totalCost = 0;
            int quantity = 0;

            foreach (var movement in movements.Where(m => m.MovementType is InventoryMovementType.Inbound or InventoryMovementType.Outbound or InventoryMovementType.Adjustment))
            {
                if (movement.Quantity > 0)
                {
                    totalCost += movement.Quantity * movement.UnitCost;
                    quantity += movement.Quantity;
                }
                else if (movement.Quantity < 0)
                {
                    var averageCost = quantity <= 0 ? 0 : totalCost / quantity;
                    totalCost += movement.Quantity * averageCost;
                    quantity += movement.Quantity;
                }
            }

            var unitCost = quantity <= 0 ? 0 : totalCost / quantity;

            return new InventoryValuationDto
            {
                ProductId = productId,
                Method = "Average",
                QuantityOnHand = Math.Max(quantity, 0),
                UnitCost = Math.Round(unitCost, 4),
                TotalValue = Math.Round(Math.Max(quantity, 0) * unitCost, 2)
            };
        }

        private static InventoryValuationDto CalculateFifo(int productId, List<Domain.Entities.InventoryMovement> movements)
        {
            var layers = new Queue<(int Qty, decimal Cost)>();

            foreach (var movement in movements.Where(m => m.MovementType is InventoryMovementType.Inbound or InventoryMovementType.Outbound or InventoryMovementType.Adjustment))
            {
                if (movement.Quantity > 0)
                {
                    layers.Enqueue((movement.Quantity, movement.UnitCost));
                    continue;
                }

                var outbound = -movement.Quantity;
                while (outbound > 0 && layers.Count > 0)
                {
                    var layer = layers.Dequeue();
                    if (layer.Qty > outbound)
                    {
                        layers.Enqueue((layer.Qty - outbound, layer.Cost));
                        outbound = 0;
                    }
                    else
                    {
                        outbound -= layer.Qty;
                    }
                }
            }

            var qty = layers.Sum(l => l.Qty);
            var value = layers.Sum(l => l.Qty * l.Cost);
            var unit = qty == 0 ? 0 : value / qty;

            return new InventoryValuationDto
            {
                ProductId = productId,
                Method = "FIFO",
                QuantityOnHand = qty,
                UnitCost = Math.Round(unit, 4),
                TotalValue = Math.Round(value, 2)
            };
        }
    }
}
