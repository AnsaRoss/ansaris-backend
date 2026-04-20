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
        private readonly GetInvoicePaymentsUseCase _getInvoicePaymentsUseCase;
        private readonly GetOutstandingInvoicesUseCase _getOutstandingInvoicesUseCase;
        private readonly GetInvoiceAgingReportUseCase _getInvoiceAgingReportUseCase;
        private readonly GetAgingByCustomerUseCase _getAgingByCustomerUseCase;
        private readonly GetCustomerStatementUseCase _getCustomerStatementUseCase;
        private readonly GetInvoiceKpiSummaryUseCase _getInvoiceKpiSummaryUseCase;

        public InvoicesController(
            CreateInvoiceUseCase createInvoiceUseCase,
            GenerateAccountingEntriesUseCase generateAccountingEntriesUseCase,
            RegisterPaymentUseCase registerPaymentUseCase,
            CancelInvoiceUseCase cancelInvoiceUseCase,
            GetInvoiceByIdUseCase getInvoiceByIdUseCase,
            GetAllInvoicesUseCase getAllInvoicesUseCase,
            GetInvoicePaymentsUseCase getInvoicePaymentsUseCase,
            GetOutstandingInvoicesUseCase getOutstandingInvoicesUseCase,
            GetInvoiceAgingReportUseCase getInvoiceAgingReportUseCase,
            GetAgingByCustomerUseCase getAgingByCustomerUseCase,
            GetCustomerStatementUseCase getCustomerStatementUseCase,
            GetInvoiceKpiSummaryUseCase getInvoiceKpiSummaryUseCase)
        {
            _createInvoiceUseCase = createInvoiceUseCase;
            _generateAccountingEntriesUseCase = generateAccountingEntriesUseCase;
            _registerPaymentUseCase = registerPaymentUseCase;
            _cancelInvoiceUseCase = cancelInvoiceUseCase;
            _getInvoiceByIdUseCase = getInvoiceByIdUseCase;
            _getAllInvoicesUseCase = getAllInvoicesUseCase;
            _getInvoicePaymentsUseCase = getInvoicePaymentsUseCase;
            _getOutstandingInvoicesUseCase = getOutstandingInvoicesUseCase;
            _getInvoiceAgingReportUseCase = getInvoiceAgingReportUseCase;
            _getAgingByCustomerUseCase = getAgingByCustomerUseCase;
            _getCustomerStatementUseCase = getCustomerStatementUseCase;
            _getInvoiceKpiSummaryUseCase = getInvoiceKpiSummaryUseCase;
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
                warehouseId = createdInvoice?.WarehouseId,
                date = createdInvoice?.Date,
                dueDate = createdInvoice?.DueDate,
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

        [HttpGet("outstanding")]
        public async Task<IActionResult> GetOutstanding([FromQuery] int? type)
        {
            ErpApp.Domain.InvoiceType? invoiceType = null;
            if (type.HasValue)
            {
                if (!Enum.IsDefined(typeof(ErpApp.Domain.InvoiceType), type.Value))
                    return BadRequest(new { error = "El parámetro type no es válido." });

                invoiceType = (ErpApp.Domain.InvoiceType)type.Value;
            }

            var result = await _getOutstandingInvoicesUseCase.ExecuteAsync(invoiceType);
            return Ok(result);
        }

        [HttpGet("aging")]
        public async Task<IActionResult> GetAging([FromQuery] int type, [FromQuery] DateTime? asOfDate, [FromQuery] int creditDays = 30)
        {
            if (!Enum.IsDefined(typeof(ErpApp.Domain.InvoiceType), type))
                return BadRequest(new { error = "El parámetro type no es válido." });

            var result = await _getInvoiceAgingReportUseCase.ExecuteAsync((ErpApp.Domain.InvoiceType)type, asOfDate, creditDays);
            return Ok(result);
        }

        [HttpGet("aging-by-customer")]
        public async Task<IActionResult> GetAgingByCustomer([FromQuery] int type, [FromQuery] DateTime? asOfDate)
        {
            if (!Enum.IsDefined(typeof(ErpApp.Domain.InvoiceType), type))
                return BadRequest(new { error = "El parámetro type no es válido." });

            var result = await _getAgingByCustomerUseCase.ExecuteAsync((ErpApp.Domain.InvoiceType)type, asOfDate);
            return Ok(result);
        }

        [HttpGet("customers/{customerId:int}/statement")]
        public async Task<IActionResult> GetCustomerStatement(int customerId, [FromQuery] int type, [FromQuery] DateTime? asOfDate)
        {
            if (!Enum.IsDefined(typeof(ErpApp.Domain.InvoiceType), type))
                return BadRequest(new { error = "El parámetro type no es válido." });

            var result = await _getCustomerStatementUseCase.ExecuteAsync(customerId, (ErpApp.Domain.InvoiceType)type, asOfDate);
            return Ok(result);
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> GetKpis()
        {
            var result = await _getInvoiceKpiSummaryUseCase.ExecuteAsync();
            return Ok(result);
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
            await _registerPaymentUseCase.ExecuteAsync(invoiceId, dto.TreasuryAccountId, dto.Amount, dto.PaymentDate, dto.Notes);
            return NoContent();
        }

        [HttpGet("{invoiceId:int}/payments")]
        public async Task<IActionResult> GetPayments(int invoiceId)
        {
            var payments = await _getInvoicePaymentsUseCase.ExecuteAsync(invoiceId);
            if (payments == null)
                return NotFound();

            return Ok(payments);
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
