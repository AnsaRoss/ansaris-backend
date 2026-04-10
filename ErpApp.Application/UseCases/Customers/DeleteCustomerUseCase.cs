using ErpApp.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.Customers
{
    public class DeleteCustomerUseCase
    {
        private readonly ICustomerRepository _repository;

        public DeleteCustomerUseCase(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            if (customer == null)
                throw new KeyNotFoundException("Cliente no encontrado");

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
    }

}
