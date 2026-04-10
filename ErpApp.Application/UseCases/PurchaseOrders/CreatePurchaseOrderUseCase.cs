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

        public CreatePurchaseOrderUseCase(IPurchaseOrderRepository repository, IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public async Task<PurchaseOrder> ExecuteAsync(PurchaseOrderCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("La orden debe contener al menos un ítem.");

            var order = new PurchaseOrder
            {
                OrderDate = dto.OrderDate == default ? DateTime.UtcNow : dto.OrderDate,
                Status = "Pending",
                Items = dto.Items.Select(i => new PurchaseOrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = (_productRepository.GetByIdAsync(i.ProductId))?.Result?.Price ?? 0
                }).ToList()
            };

            await _repository.AddAsync(order);
            await _repository.SaveChangesAsync();

            order.OrderNumber = $"PO-{order.OrderDate:yyyyMMdd}-{order.Id:D5}";

            await _repository.UpdateAsync(order);
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
