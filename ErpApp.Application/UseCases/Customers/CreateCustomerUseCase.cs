using ErpApp.Application.Dtos.Customers;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.Customers
{
    public class CreateCustomerUseCase
    {
        private readonly ICustomerRepository _repository;

        public CreateCustomerUseCase(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Customer> ExecuteAsync(CustomerCreateDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address
            };

            await _repository.AddAsync(customer);
            await _repository.SaveChangesAsync();

            return customer;
        }
        
    }

}
