using ErpApp.Application.Dtos.SaleOrders;
using ErpApp.Application.UseCases.SaleOrders;
using Microsoft.AspNetCore.Mvc;

namespace ErpApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrdersController : ControllerBase
    {
        private readonly CreateSalesOrderUseCase _createSalesOrderUseCase;
        private readonly UpdateSalesOrderStatusUseCase _updateStatusUseCase;
        private readonly GetSalesOrderByIdUseCase _getByIdUseCase;
        private readonly GetAllSalesOrdersUseCase _getAllUseCase;

        public SalesOrdersController(
            CreateSalesOrderUseCase createSalesOrderUseCase, 
            UpdateSalesOrderStatusUseCase updateStatusUseCase,
            GetSalesOrderByIdUseCase getByIdUseCase,
            GetAllSalesOrdersUseCase getAllUseCase)
        {
            _createSalesOrderUseCase = createSalesOrderUseCase;
            _updateStatusUseCase = updateStatusUseCase;
            _getByIdUseCase = getByIdUseCase;
            _getAllUseCase = getAllUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalesOrderCreateDto dto)
        {
            try
            {
                var order = await _createSalesOrderUseCase.ExecuteAsync(dto);
                return CreatedAtAction(nameof(GetById), new { orderNumber = order.OrderNumber }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _getAllUseCase.ExecuteAsync();
            return Ok(orders);
        }

        [HttpGet("{orderNumber}")]
        public async Task<IActionResult> GetById(string orderNumber)
        {
            var order = await _getByIdUseCase.ExecuteAsync(orderNumber);
            if (order == null) return NotFound();
            return Ok(order);
        }
        [HttpPut("{orderNumber}/status")]
        public async Task<IActionResult> UpdateStatus(string orderNumber, [FromBody] UpdateSalesOrderStatusDto dto)
        {
            try
            {
                var result = await _updateStatusUseCase.ExecuteAsync(orderNumber,dto.Status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}
