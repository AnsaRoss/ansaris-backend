using ErpApp.Application.Dtos.PurchaseOrders;
using ErpApp.Application.UseCases.PurchaseOrders;
using Microsoft.AspNetCore.Mvc;

namespace ErpApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly CreatePurchaseOrderUseCase _createUseCase;
        private readonly GetPurchaseOrderByIdUseCase _getByIdUseCase;
        private readonly GetAllPurchaseOrdersUseCase _getAllUseCase;
        private readonly ReceivePurchaseOrderUseCase _receiveUseCase;

        public PurchaseOrdersController(
            CreatePurchaseOrderUseCase createUseCase,
            GetPurchaseOrderByIdUseCase getByIdUseCase,
            GetAllPurchaseOrdersUseCase getAllUseCase,
            ReceivePurchaseOrderUseCase receiveUseCase)
        {
            _createUseCase = createUseCase;
            _getByIdUseCase = getByIdUseCase;
            _getAllUseCase = getAllUseCase;
            _receiveUseCase = receiveUseCase;
        }

        // POST: api/PurchaseOrders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseOrderCreateDto dto)
        {
            try
            {
                var createdOrder = await _createUseCase.ExecuteAsync(dto);
                return CreatedAtAction(nameof(GetById), new { orderNumber = createdOrder.OrderNumber }, createdOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/PurchaseOrders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _getAllUseCase.ExecuteAsync();
            return Ok(orders);
        }

        // GET: api/PurchaseOrders/5
        [HttpGet("{orderNumber}")]
        public async Task<IActionResult> GetById(string orderNumber)
        {
            var order = await _getByIdUseCase.ExecuteAsync(orderNumber);
            if (order == null) return NotFound();
            return Ok(order);
        }

        // PUT: api/PurchaseOrders/5/receive
        [HttpPut("{orderNumber}/receive")]
        public async Task<IActionResult> Receive(string orderNumber)
        {
            try
            {
                var updatedOrder = await _receiveUseCase.ExecuteAsync(orderNumber);
                if (updatedOrder == null)
                    return NotFound();

                return Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
