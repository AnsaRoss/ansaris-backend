using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.PurchaseOrders
{
    public class CancelPurchaseOrderUseCase
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public CancelPurchaseOrderUseCase(IPurchaseOrderRepository purchaseOrderRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task ExecuteAsync(string orderNumber, string? reason)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(orderNumber);
            if (order == null)
                throw new Exception("Orden no encontrada.");

            if (order.Status == "Received")
                throw new Exception("No se puede anular una orden ya recibida.");

            if (order.Status == "Cancelled")
                throw new Exception("La orden ya está anulada.");

            order.Status = "Cancelled";
            await _purchaseOrderRepository.UpdateAsync(order);
        }
    }
}
