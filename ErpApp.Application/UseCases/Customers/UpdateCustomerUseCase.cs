using ErpApp.Domain.Ports;
using ErpApp.Application.Dtos.Customers;
using ErpApp.Domain.Entities;

namespace ErpApp.Application.UseCases.Customers
{
    public class UpdateCustomerUseCase
    {
        private readonly ICustomerRepository _repository;
        public UpdateCustomerUseCase(ICustomerRepository repository)
        {
            _repository = repository;
        }
        public async Task<Customer> ExecuteAsync(int id, CustomerUpdateDto dto)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null)
                throw new KeyNotFoundException("Cliente no encontrado");

            customer.Name = dto.Name;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.Address = dto.Address;

            await _repository.UpdateAsync(customer);
            await _repository.SaveChangesAsync();

            return customer;
        }

    }

}
