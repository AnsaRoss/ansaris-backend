using ErpApp.Application.Dtos.Invoice;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class GetInvoicePaymentsUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ITreasuryMovementRepository _treasuryMovementRepository;

        public GetInvoicePaymentsUseCase(
            IInvoiceRepository invoiceRepository,
            ITreasuryMovementRepository treasuryMovementRepository)
        {
            _invoiceRepository = invoiceRepository;
            _treasuryMovementRepository = treasuryMovementRepository;
        }

        public async Task<List<InvoicePaymentReadDto>?> ExecuteAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
                return null;

            var invoiceReference = invoice.InvoiceNumber ?? invoice.Id.ToString();
            var movements = await _treasuryMovementRepository.GetByReferenceAsync("InvoicePayment", invoiceReference);

            return movements.Select(m => new InvoicePaymentReadDto
            {
                TreasuryMovementId = m.Id,
                TreasuryAccountId = m.TreasuryAccountId,
                TreasuryAccountCode = m.TreasuryAccount.Code,
                PaymentDate = m.MovementDate,
                Amount = m.Amount,
                Notes = m.Notes
            }).ToList();
        }
    }
}
