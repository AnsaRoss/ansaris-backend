using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.SaleOrders
{
    public class GetSalesOrderByIdUseCase
    {
        private readonly ISalesOrderRepository _repository;

        public GetSalesOrderByIdUseCase(ISalesOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<SalesOrder?> ExecuteAsync(string orderNumber)
        {
            return await _repository.GetByIdAsync(orderNumber);
        }
    }
}
