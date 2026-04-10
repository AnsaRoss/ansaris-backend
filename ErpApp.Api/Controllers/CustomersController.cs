using ErpApp.Application.Dtos.Customers;
using ErpApp.Application.UseCases.Customers;
using ErpApp.Domain.Ports;
using Microsoft.AspNetCore.Mvc;
using static ErpApp.Application.UseCases.Customers.CreateCustomerUseCase;

namespace ErpApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CreateCustomerUseCase _createUseCase;
        private readonly UpdateCustomerUseCase _updateUseCase;
        private readonly GetCustomerByIdUseCase _getByIdUseCase;
        private readonly GetAllCustomersUseCase _getAllUseCase;
        private readonly DeleteCustomerUseCase _deleteUseCase;

        public CustomersController(
            CreateCustomerUseCase createUseCase,
            UpdateCustomerUseCase updateUseCase,
            GetCustomerByIdUseCase getByIdUseCase,
            GetAllCustomersUseCase getAllUseCase,
            DeleteCustomerUseCase deleteUseCase)
        {
            _createUseCase = createUseCase;
            _updateUseCase = updateUseCase;
            _getByIdUseCase = getByIdUseCase;
            _getAllUseCase = getAllUseCase;
            _deleteUseCase = deleteUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
        {
            var customer = await _createUseCase.ExecuteAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> GetById(int id, [FromBody] CustomerUpdateDto dto)
        {
            try
            {
                var customer = await _updateUseCase.ExecuteAsync(id, dto);
                return Ok(customer);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var customer = await _getByIdUseCase.ExecuteAsync(id);
                return Ok(customer);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _getAllUseCase.ExecuteAsync();
            return Ok(customers);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _deleteUseCase.ExecuteAsync(id);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


    }

}
