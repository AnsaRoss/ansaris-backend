using ErpApp.Application.Dtos.SaleOrders;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.SaleOrders
{
    public class CreateSalesOrderUseCase
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly IProductRepository _productRepository;

        public CreateSalesOrderUseCase(
            ISalesOrderRepository salesOrderRepository,
            IProductRepository productRepository)
        {
            _salesOrderRepository = salesOrderRepository;
            _productRepository = productRepository;
        }

        public async Task<SalesOrder> ExecuteAsync(SalesOrderCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new ArgumentException("La orden debe contener al menos un ítem.");

            var order = new SalesOrder
            {
                OrderDate = dto.OrderDate == default ? DateTime.UtcNow : dto.OrderDate,
                Status = "Pending",
                CustomerId = dto.CustomerId,
                Items = dto.Items.Select(i => new SalesOrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = (_productRepository.GetByIdAsync(i.ProductId))?.Result?.Price ?? 0
                }).ToList()
            };

            await _salesOrderRepository.AddAsync(order);
            await _salesOrderRepository.SaveChangesAsync();

            // Generar número de orden tipo: SO-20250825-00001
            order.OrderNumber = $"SO-{order.OrderDate:yyyyMMdd}-{order.Id:D5}";
            await _salesOrderRepository.UpdateAsync(order);
            await _salesOrderRepository.SaveChangesAsync();

            return order;
        }
        private string GenerateOrderNumber()
        {
            var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
            var randomPart = new Random().Next(100, 999);
            return $"SO-{datePart}-{randomPart}";
        }

    }

}
