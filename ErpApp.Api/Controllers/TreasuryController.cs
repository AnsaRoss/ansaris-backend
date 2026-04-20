using ErpApp.Application.Dtos.Treasury;
using ErpApp.Application.UseCases.Treasury;
using Microsoft.AspNetCore.Mvc;

namespace ErpApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TreasuryController : ControllerBase
    {
        private readonly CreateTreasuryAccountUseCase _createTreasuryAccountUseCase;
        private readonly GetTreasuryAccountsUseCase _getTreasuryAccountsUseCase;
        private readonly RegisterTreasuryMovementUseCase _registerTreasuryMovementUseCase;
        private readonly RegisterTreasuryTransferUseCase _registerTreasuryTransferUseCase;
        private readonly GetTreasuryMovementsUseCase _getTreasuryMovementsUseCase;

        public TreasuryController(
            CreateTreasuryAccountUseCase createTreasuryAccountUseCase,
            GetTreasuryAccountsUseCase getTreasuryAccountsUseCase,
            RegisterTreasuryMovementUseCase registerTreasuryMovementUseCase,
            RegisterTreasuryTransferUseCase registerTreasuryTransferUseCase,
            GetTreasuryMovementsUseCase getTreasuryMovementsUseCase)
        {
            _createTreasuryAccountUseCase = createTreasuryAccountUseCase;
            _getTreasuryAccountsUseCase = getTreasuryAccountsUseCase;
            _registerTreasuryMovementUseCase = registerTreasuryMovementUseCase;
            _registerTreasuryTransferUseCase = registerTreasuryTransferUseCase;
            _getTreasuryMovementsUseCase = getTreasuryMovementsUseCase;
        }

        [HttpGet("accounts")]
        [ProducesResponseType(typeof(List<TreasuryAccountDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccounts()
        {
            var result = await _getTreasuryAccountsUseCase.ExecuteAsync();
            return Ok(result);
        }

        [HttpPost("accounts")]
        [ProducesResponseType(typeof(TreasuryAccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccount([FromBody] TreasuryAccountCreateDto dto)
        {
            try
            {
                var result = await _createTreasuryAccountUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("movements")]
        [ProducesResponseType(typeof(List<TreasuryMovementDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMovements([FromQuery] int? treasuryAccountId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var result = await _getTreasuryMovementsUseCase.ExecuteAsync(treasuryAccountId, from, to);
            return Ok(result);
        }

        [HttpPost("movements")]
        [ProducesResponseType(typeof(TreasuryMovementDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterMovement([FromBody] RegisterTreasuryMovementDto dto)
        {
            try
            {
                var result = await _registerTreasuryMovementUseCase.ExecuteAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("transfers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterTransfer([FromBody] TreasuryTransferDto dto)
        {
            try
            {
                await _registerTreasuryTransferUseCase.ExecuteAsync(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
