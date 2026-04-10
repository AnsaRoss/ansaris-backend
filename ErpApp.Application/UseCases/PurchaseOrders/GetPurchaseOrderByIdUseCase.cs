using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.PurchaseOrders
{
    public class GetPurchaseOrderByIdUseCase
    {
        private readonly IPurchaseOrderRepository _repository;

        public GetPurchaseOrderByIdUseCase(IPurchaseOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<PurchaseOrder?> ExecuteAsync(string orderNumber)
        {
            return await _repository.GetByIdAsync(orderNumber);
        }
    }

}
