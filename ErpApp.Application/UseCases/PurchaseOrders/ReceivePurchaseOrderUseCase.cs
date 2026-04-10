using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
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

        public ReceivePurchaseOrderUseCase(IPurchaseOrderRepository purchaseRepo, IProductRepository productRepo)
        {
            _purchaseRepo = purchaseRepo;
            _productRepo = productRepo;
        }

        public async Task<PurchaseOrder> ExecuteAsync(string orderNumber)
        {
            var order = await _purchaseRepo.GetByIdAsync(orderNumber);
            if (order == null) throw new Exception("Orden no encontrada.");
            if (order.Status == "Received") throw new Exception("La orden ya fue recibida.");

            // Aumentar stock
            foreach (var item in order.Items)
            {
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product == null) continue;

                product.Stock += item.Quantity;
            }

            order.Status = "Received";
            await _purchaseRepo.SaveChangesAsync();
            await _productRepo.SaveChangesAsync();
            return order;
        }
    }

}
