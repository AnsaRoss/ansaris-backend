using ErpApp.Application.Dtos.Invoice;
using ErpApp.Application.Dtos.Invoice;
using ErpApp.Domain;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class GetInvoiceAgingReportUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoiceAgingReportUseCase(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<InvoiceAgingReportDto> ExecuteAsync(InvoiceType type, DateTime? asOfDate, int creditDays)
        {
            var normalizedCreditDays = creditDays < 0 ? 0 : creditDays;
            var reportDate = (asOfDate ?? DateTime.UtcNow).Date;

            var invoices = await _invoiceRepository.GetAllAsync();
            var openInvoices = invoices
                .Where(i => i.Type == type)
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .Where(i => i.TotalAmount > i.PaidAmount);

            var report = new InvoiceAgingReportDto
            {
                AsOfDate = reportDate,
                CreditDays = normalizedCreditDays
            };

            foreach (var invoice in openInvoices)
            {
                var pending = invoice.TotalAmount - invoice.PaidAmount;
                var dueDate = invoice.DueDate == default
                    ? invoice.Date.Date.AddDays(normalizedCreditDays)
                    : invoice.DueDate.Date;
                var overdueDays = (reportDate - dueDate).Days;

                if (overdueDays <= 0)
                {
                    report.Current += pending;
                }
                else if (overdueDays <= 30)
                {
                    report.Bucket1To30 += pending;
                }
                else if (overdueDays <= 60)
                {
                    report.Bucket31To60 += pending;
                }
                else if (overdueDays <= 90)
                {
                    report.Bucket61To90 += pending;
                }
                else
                {
                    report.BucketOver90 += pending;
                }
            }

            report.TotalOutstanding = report.Current
                + report.Bucket1To30
                + report.Bucket31To60
                + report.Bucket61To90
                + report.BucketOver90;

            return report;
        }
    }
}
