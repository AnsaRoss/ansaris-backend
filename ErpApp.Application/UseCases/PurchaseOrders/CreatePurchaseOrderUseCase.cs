using ErpApp.Application.Dtos.PurchaseOrders;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.PurchaseOrders
{
    public class CreatePurchaseOrderUseCase
    {
        private readonly IPurchaseOrderRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;

        public CreatePurchaseOrderUseCase(
            IPurchaseOrderRepository repository,
            IProductRepository productRepository,
            IWarehouseRepository warehouseRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
        }

        public async Task<PurchaseOrder> ExecuteAsync(PurchaseOrderCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("La orden debe contener al menos un ítem.");

            if (dto.TaxRate < 0 || dto.TaxRate > 100)
                throw new ArgumentException("La tasa de impuesto debe estar entre 0 y 100.");

            if (dto.WarehouseId <= 0)
                throw new ArgumentException("WarehouseId es obligatorio.");

            var warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId)
                ?? throw new ArgumentException($"Almacén con ID {dto.WarehouseId} no encontrado.");

            if (!warehouse.IsActive)
                throw new ArgumentException("El almacén seleccionado está inactivo.");

            var orderDate = dto.OrderDate == default ? DateTime.UtcNow : dto.OrderDate;
            var series = string.IsNullOrWhiteSpace(dto.Series) ? "PO" : dto.Series.Trim().ToUpperInvariant();
            var sequenceNumber = await _repository.GetNextSequenceNumberAsync(series, orderDate);

            var details = new List<PurchaseOrderDetail>();
            decimal subtotalAmount = 0;

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    throw new ArgumentException($"Cantidad inválida para el producto {item.ProductId}.");

                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Producto con ID {item.ProductId} no encontrado.");

                subtotalAmount += product.Price * item.Quantity;

                details.Add(new PurchaseOrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }

            if (dto.DiscountAmount < 0)
                throw new ArgumentException("El descuento no puede ser negativo.");

            if (dto.DiscountAmount > subtotalAmount)
                throw new ArgumentException("El descuento no puede ser mayor al subtotal.");

            var taxableBase = subtotalAmount - dto.DiscountAmount;
            var taxAmount = Math.Round(taxableBase * (dto.TaxRate / 100m), 2);
            var totalAmount = taxableBase + taxAmount;

            var order = new PurchaseOrder
            {
                OrderDate = orderDate,
                WarehouseId = warehouse.Id,
                Status = "Pending",
                Series = series,
                SequenceNumber = sequenceNumber,
                OrderNumber = $"{series}-{orderDate:yyyyMMdd}-{sequenceNumber:D6}",
                SubtotalAmount = subtotalAmount,
                DiscountAmount = dto.DiscountAmount,
                TaxRate = dto.TaxRate,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                Items = details
            };

            await _repository.AddAsync(order);
            await _repository.SaveChangesAsync();
            return order;
        }
        private string GenerateOrderNumber()
        {
            // Puedes hacer algo más complejo, aquí un ejemplo básico con fecha y un random
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(100, 999);
            return $"PO-{datePart}-{randomPart}";
        }
    }

}
