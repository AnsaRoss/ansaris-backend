using ErpApp.Application.Dtos.Invoice;
using ErpApp.Application.UseCases.Invoice;
using ErpApp.Application.UseCases.Invoices;
using Microsoft.AspNetCore.Mvc;

namespace ErpApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly CreateInvoiceUseCase _createInvoiceUseCase;
        private readonly GenerateAccountingEntriesUseCase _generateAccountingEntriesUseCase;
        private readonly RegisterPaymentUseCase _registerPaymentUseCase;

        public InvoicesController(
            CreateInvoiceUseCase createInvoiceUseCase,
            GenerateAccountingEntriesUseCase generateAccountingEntriesUseCase,
            RegisterPaymentUseCase registerPaymentUseCase)
        {
            _createInvoiceUseCase = createInvoiceUseCase;
            _generateAccountingEntriesUseCase = generateAccountingEntriesUseCase;
            _registerPaymentUseCase = registerPaymentUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
        {
            var invoiceId = await _createInvoiceUseCase.ExecuteAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = invoiceId }, null);
        }

        [HttpPost("{invoiceId}/generate-entries")]
        public async Task<IActionResult> GenerateEntries(int invoiceId)
        {
            await _generateAccountingEntriesUseCase.ExecuteAsync(invoiceId);
            return NoContent();
        }

        [HttpPost("{invoiceId}/register-payment")]
        public async Task<IActionResult> RegisterPayment(int invoiceId, [FromBody] RegisterPaymentDto dto)
        {
            await _registerPaymentUseCase.ExecuteAsync(invoiceId, dto.Amount, dto.PaymentDate);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            // Aquí podrías llamar a un caso de uso para obtener factura por id.
            // Por ahora, solo un ejemplo ficticio:
            return Ok(/* resultado del caso de uso */);
        }
    }
}
