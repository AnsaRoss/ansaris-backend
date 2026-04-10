using ErpApp.Application.Dtos.Invoice;
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
        private readonly CancelInvoiceUseCase _cancelInvoiceUseCase;
        private readonly GetInvoiceByIdUseCase _getInvoiceByIdUseCase;
        private readonly GetAllInvoicesUseCase _getAllInvoicesUseCase;

        public InvoicesController(
            CreateInvoiceUseCase createInvoiceUseCase,
            GenerateAccountingEntriesUseCase generateAccountingEntriesUseCase,
            RegisterPaymentUseCase registerPaymentUseCase,
            CancelInvoiceUseCase cancelInvoiceUseCase,
            GetInvoiceByIdUseCase getInvoiceByIdUseCase,
            GetAllInvoicesUseCase getAllInvoicesUseCase)
        {
            _createInvoiceUseCase = createInvoiceUseCase;
            _generateAccountingEntriesUseCase = generateAccountingEntriesUseCase;
            _registerPaymentUseCase = registerPaymentUseCase;
            _cancelInvoiceUseCase = cancelInvoiceUseCase;
            _getInvoiceByIdUseCase = getInvoiceByIdUseCase;
            _getAllInvoicesUseCase = getAllInvoicesUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
        {
            var invoiceId = await _createInvoiceUseCase.ExecuteAsync(dto);
            var createdInvoice = await _getInvoiceByIdUseCase.ExecuteAsync(invoiceId);

            return CreatedAtAction(nameof(GetById), new { id = invoiceId }, new
            {
                id = createdInvoice?.Id ?? invoiceId,
                invoiceNumber = createdInvoice?.InvoiceNumber,
                series = createdInvoice?.Series,
                date = createdInvoice?.Date,
                subtotalAmount = createdInvoice?.SubtotalAmount,
                discountAmount = createdInvoice?.DiscountAmount,
                taxRate = createdInvoice?.TaxRate,
                taxAmount = createdInvoice?.TaxAmount,
                totalAmount = createdInvoice?.TotalAmount,
                paidAmount = createdInvoice?.PaidAmount,
                status = createdInvoice?.Status.ToString()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _getAllInvoicesUseCase.ExecuteAsync();
            return Ok(invoices);
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

        [HttpPost("{invoiceId:int}/cancel")]
        public async Task<IActionResult> Cancel(int invoiceId, [FromBody] CancelInvoiceDto dto)
        {
            await _cancelInvoiceUseCase.ExecuteAsync(invoiceId, dto?.Reason);
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _getInvoiceByIdUseCase.ExecuteAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }
    }
}
