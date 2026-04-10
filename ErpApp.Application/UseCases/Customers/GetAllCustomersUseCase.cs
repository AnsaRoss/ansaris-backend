using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.Customers
{
    public class GetAllCustomersUseCase
    {
        private readonly ICustomerRepository _repository;

        public GetAllCustomersUseCase(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Customer>> ExecuteAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
