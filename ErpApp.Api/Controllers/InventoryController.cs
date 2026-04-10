using ErpApp.Application.Dtos.Inventory;
using ErpApp.Application.UseCases.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace ErpApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly GetInventoryStockUseCase _getInventoryStockUseCase;
        private readonly GetInventoryMovementsByProductUseCase _getInventoryMovementsByProductUseCase;
        private readonly GetKardexUseCase _getKardexUseCase;
        private readonly ReserveStockUseCase _reserveStockUseCase;
        private readonly ReleaseStockUseCase _releaseStockUseCase;
        private readonly ReverseInventoryMovementUseCase _reverseInventoryMovementUseCase;
        private readonly GetInventoryValuationUseCase _getInventoryValuationUseCase;
        private readonly CreateWarehouseUseCase _createWarehouseUseCase;
        private readonly GetWarehousesUseCase _getWarehousesUseCase;
        private readonly GetWarehouseStockUseCase _getWarehouseStockUseCase;
        private readonly ExecuteInventoryTransferUseCase _executeInventoryTransferUseCase;
        private readonly GetInventoryTransfersUseCase _getInventoryTransfersUseCase;
        private readonly AdjustInventoryUseCase _adjustInventoryUseCase;

        public InventoryController(
            GetInventoryStockUseCase getInventoryStockUseCase,
            GetInventoryMovementsByProductUseCase getInventoryMovementsByProductUseCase,
            GetKardexUseCase getKardexUseCase,
            ReserveStockUseCase reserveStockUseCase,
            ReleaseStockUseCase releaseStockUseCase,
            ReverseInventoryMovementUseCase reverseInventoryMovementUseCase,
            GetInventoryValuationUseCase getInventoryValuationUseCase,
            CreateWarehouseUseCase createWarehouseUseCase,
            GetWarehousesUseCase getWarehousesUseCase,
            GetWarehouseStockUseCase getWarehouseStockUseCase,
            ExecuteInventoryTransferUseCase executeInventoryTransferUseCase,
            GetInventoryTransfersUseCase getInventoryTransfersUseCase,
            AdjustInventoryUseCase adjustInventoryUseCase)
        {
            _getInventoryStockUseCase = getInventoryStockUseCase;
            _getInventoryMovementsByProductUseCase = getInventoryMovementsByProductUseCase;
            _getKardexUseCase = getKardexUseCase;
            _reserveStockUseCase = reserveStockUseCase;
            _releaseStockUseCase = releaseStockUseCase;
            _reverseInventoryMovementUseCase = reverseInventoryMovementUseCase;
            _getInventoryValuationUseCase = getInventoryValuationUseCase;
            _createWarehouseUseCase = createWarehouseUseCase;
            _getWarehousesUseCase = getWarehousesUseCase;
            _getWarehouseStockUseCase = getWarehouseStockUseCase;
            _executeInventoryTransferUseCase = executeInventoryTransferUseCase;
            _getInventoryTransfersUseCase = getInventoryTransfersUseCase;
            _adjustInventoryUseCase = adjustInventoryUseCase;
        }

        [HttpGet("stock")]
        public async Task<IActionResult> GetStock()
        {
            var result = await _getInventoryStockUseCase.ExecuteAsync();
            return Ok(result);
        }

        [HttpGet("warehouses")]
        [ProducesResponseType(typeof(List<WarehouseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWarehouses()
        {
            var result = await _getWarehousesUseCase.ExecuteAsync();
            return Ok(result);
        }

        [HttpGet("warehouses/{warehouseId:int}/stock")]
        [ProducesResponseType(typeof(List<WarehouseStockDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWarehouseStock(int warehouseId)
        {
            try
            {
                var result = await _getWarehouseStockUseCase.ExecuteAsync(warehouseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("warehouses")]
        [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWarehouse([FromBody] WarehouseCreateDto dto, [FromHeader(Name = "X-Role")] string? role)
        {
            if (!HasInventoryWritePermission(role))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "No tienes permisos para esta operación." });

            try
            {
                var result = await _createWarehouseUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("transfers")]
        [ProducesResponseType(typeof(List<InventoryTransferResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransfers()
        {
            var result = await _getInventoryTransfersUseCase.ExecuteAsync();
            return Ok(result);
        }

        [HttpPost("transfers")]
        [ProducesResponseType(typeof(InventoryTransferResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExecuteTransfer([FromBody] InventoryTransferCreateDto dto, [FromHeader(Name = "X-Role")] string? role)
        {
            if (!HasInventoryWritePermission(role))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "No tienes permisos para esta operación." });

            try
            {
                var result = await _executeInventoryTransferUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("products/{productId:int}/movements")]
        public async Task<IActionResult> GetMovements(int productId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var result = await _getInventoryMovementsByProductUseCase.ExecuteAsync(productId, from, to);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("kardex")]
        [ProducesResponseType(typeof(KardexPagedResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetKardex([FromQuery] KardexQueryDto dto)
        {
            var result = await _getKardexUseCase.ExecuteAsync(dto);
            return Ok(result);
        }

        [HttpGet("valuation/{productId:int}")]
        [ProducesResponseType(typeof(InventoryValuationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetValuation(int productId, [FromQuery] string method = "average")
        {
            try
            {
                var result = await _getInventoryValuationUseCase.ExecuteAsync(productId, method);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("adjustments")]
        [ProducesResponseType(typeof(AdjustInventoryResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Adjust([FromBody] AdjustInventoryDto dto, [FromHeader(Name = "X-Role")] string? role)
        {
            if (!HasInventoryWritePermission(role))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "No tienes permisos para esta operación." });

            try
            {
                var result = await _adjustInventoryUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("reservations")]
        [ProducesResponseType(typeof(AdjustInventoryResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Reserve([FromBody] ReserveStockDto dto, [FromHeader(Name = "X-Role")] string? role)
        {
            if (!HasInventoryWritePermission(role))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "No tienes permisos para esta operación." });

            try
            {
                var result = await _reserveStockUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("releases")]
        [ProducesResponseType(typeof(AdjustInventoryResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Release([FromBody] ReleaseStockDto dto, [FromHeader(Name = "X-Role")] string? role)
        {
            if (!HasInventoryWritePermission(role))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "No tienes permisos para esta operación." });

            try
            {
                var result = await _releaseStockUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("movements/{movementId:int}/reverse")]
        [ProducesResponseType(typeof(AdjustInventoryResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReverseMovement(int movementId, [FromBody] ReverseInventoryMovementDto dto, [FromHeader(Name = "X-Role")] string? role)
        {
            if (!HasInventoryWritePermission(role))
                return StatusCode(StatusCodes.Status403Forbidden, new { error = "No tienes permisos para esta operación." });

            try
            {
                var result = await _reverseInventoryMovementUseCase.ExecuteAsync(movementId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private static bool HasInventoryWritePermission(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return false;

            return role.Equals("admin", StringComparison.OrdinalIgnoreCase)
                || role.Equals("inventory_manager", StringComparison.OrdinalIgnoreCase);
        }
    }
}
