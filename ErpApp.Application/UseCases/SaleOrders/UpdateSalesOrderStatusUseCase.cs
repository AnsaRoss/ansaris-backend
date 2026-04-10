using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.SaleOrders
{
    public class UpdateSalesOrderStatusUseCase
    {
        private readonly ISalesOrderRepository _salesOrderRepository;
        private readonly IProductRepository _productRepository;

        public UpdateSalesOrderStatusUseCase(
            ISalesOrderRepository salesOrderRepository,
            IProductRepository productRepository)
        {
            _salesOrderRepository = salesOrderRepository;
            _productRepository = productRepository;
        }

        public async Task<SalesOrder> ExecuteAsync(string orderId, string Status)
        {
            var order = await _salesOrderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new ArgumentException("La orden no existe.");

            if (order.Status == "Delivered")
                throw new InvalidOperationException("La orden ya fue entregada.");

            if (Status == "Delivered")
            {
                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);

                    if (product == null)
                        throw new InvalidOperationException($"Producto con ID {item.ProductId} no encontrado.");

                    if (product.Stock < item.Quantity)
                        throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}.");

                    product.Stock -= item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            order.Status = Status;
            await _salesOrderRepository.UpdateAsync(order);
            await _salesOrderRepository.SaveChangesAsync();

            return order;
        }
    }

}
