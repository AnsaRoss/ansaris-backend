using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.Customers
{
    public class GetCustomerByIdUseCase
    {
        private readonly ICustomerRepository _repository;

        public GetCustomerByIdUseCase(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Customer> ExecuteAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null)
                throw new KeyNotFoundException("Cliente no encontrado");

            return customer;
        }
    }
}
