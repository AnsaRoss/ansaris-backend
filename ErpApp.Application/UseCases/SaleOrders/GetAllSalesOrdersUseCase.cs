using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.SaleOrders
{
    public class GetAllSalesOrdersUseCase
    {
        private readonly ISalesOrderRepository _repository;

        public GetAllSalesOrdersUseCase(ISalesOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SalesOrder>> ExecuteAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
