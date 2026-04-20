using ErpApp.Application.Dtos.Invoice;
using ErpApp.Domain;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class GetInvoiceKpiSummaryUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoiceKpiSummaryUseCase(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<InvoiceKpiSummaryDto> ExecuteAsync()
        {
            var invoices = (await _invoiceRepository.GetAllAsync()).ToList();

            var outstandingNonCancelled = invoices
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .Select(i => Math.Max(i.TotalAmount - i.PaidAmount, 0m))
                .Sum();

            var salesOutstanding = invoices
                .Where(i => i.Type == InvoiceType.Sale)
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .Select(i => Math.Max(i.TotalAmount - i.PaidAmount, 0m))
                .Sum();

            var purchaseOutstanding = invoices
                .Where(i => i.Type == InvoiceType.Purchase)
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .Select(i => Math.Max(i.TotalAmount - i.PaidAmount, 0m))
                .Sum();

            return new InvoiceKpiSummaryDto
            {
                TotalInvoices = invoices.Count,
                PendingInvoices = invoices.Count(i => i.Status == InvoiceStatus.Pending),
                PaidInvoices = invoices.Count(i => i.Status == InvoiceStatus.Paid),
                CancelledInvoices = invoices.Count(i => i.Status == InvoiceStatus.Cancelled),
                TotalBilledAmount = invoices.Sum(i => i.TotalAmount),
                TotalCollectedOrPaidAmount = invoices.Sum(i => i.PaidAmount),
                TotalOutstandingAmount = outstandingNonCancelled,
                SalesOutstandingAmount = salesOutstanding,
                PurchaseOutstandingAmount = purchaseOutstanding
            };
        }
    }
}
