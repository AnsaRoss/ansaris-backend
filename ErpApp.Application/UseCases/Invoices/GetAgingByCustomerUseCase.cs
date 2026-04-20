using ErpApp.Application.Dtos.Invoice;
using ErpApp.Domain;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class GetAgingByCustomerUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetAgingByCustomerUseCase(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<List<CustomerAgingSummaryDto>> ExecuteAsync(InvoiceType type, DateTime? asOfDate)
        {
            var reportDate = (asOfDate ?? DateTime.UtcNow).Date;
            var invoices = await _invoiceRepository.GetAllAsync();

            var openInvoices = invoices
                .Where(i => i.Type == type)
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .Where(i => i.TotalAmount > i.PaidAmount)
                .Where(i => i.Date.Date <= reportDate);

            var result = new List<CustomerAgingSummaryDto>();

            foreach (var group in openInvoices.GroupBy(i => i.CustomerId).OrderBy(g => g.Key))
            {
                var row = new CustomerAgingSummaryDto
                {
                    CustomerId = group.Key
                };

                foreach (var invoice in group)
                {
                    var pending = invoice.TotalAmount - invoice.PaidAmount;
                    var dueDate = invoice.DueDate == default ? invoice.Date.Date : invoice.DueDate.Date;
                    var overdueDays = (reportDate - dueDate).Days;

                    if (overdueDays <= 0)
                    {
                        row.Current += pending;
                    }
                    else if (overdueDays <= 30)
                    {
                        row.Bucket1To30 += pending;
                    }
                    else if (overdueDays <= 60)
                    {
                        row.Bucket31To60 += pending;
                    }
                    else if (overdueDays <= 90)
                    {
                        row.Bucket61To90 += pending;
                    }
                    else
                    {
                        row.BucketOver90 += pending;
                    }
                }

                row.TotalOutstanding = row.Current
                    + row.Bucket1To30
                    + row.Bucket31To60
                    + row.Bucket61To90
                    + row.BucketOver90;

                result.Add(row);
            }

            return result;
        }
    }
}
