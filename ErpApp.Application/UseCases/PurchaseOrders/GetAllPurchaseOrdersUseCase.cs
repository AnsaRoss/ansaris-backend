using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.PurchaseOrders
{
    public class GetAllPurchaseOrdersUseCase
    {
        private readonly IPurchaseOrderRepository _repository;

        public GetAllPurchaseOrdersUseCase(IPurchaseOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PurchaseOrder>> ExecuteAsync()
        {
            return await _repository.GetAllAsync();
        }
    }

}
