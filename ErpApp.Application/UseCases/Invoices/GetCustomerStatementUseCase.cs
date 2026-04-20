using ErpApp.Application.Dtos.Invoice;
using ErpApp.Domain;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class GetCustomerStatementUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetCustomerStatementUseCase(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<CustomerStatementDto> ExecuteAsync(int customerId, InvoiceType type, DateTime? asOfDate)
        {
            var reportDate = (asOfDate ?? DateTime.UtcNow).Date;
            var invoices = await _invoiceRepository.GetAllAsync();

            var customerInvoices = invoices
                .Where(i => i.CustomerId == customerId)
                .Where(i => i.Type == type)
                .Where(i => i.Date.Date <= reportDate)
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .OrderBy(i => i.Date)
                .ToList();

            var details = customerInvoices.Select(i =>
            {
                var pending = Math.Max(i.TotalAmount - i.PaidAmount, 0m);
                var dueDate = i.DueDate == default ? i.Date.Date : i.DueDate.Date;
                var daysPastDue = dueDate >= reportDate
                    ? 0
                    : (reportDate - dueDate).Days;

                return new CustomerStatementInvoiceDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber ?? i.Id.ToString(),
                    Date = i.Date,
                    DueDate = dueDate,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    PendingAmount = pending,
                    DaysPastDue = daysPastDue,
                    Status = i.Status.ToString()
                };
            }).ToList();

            return new CustomerStatementDto
            {
                CustomerId = customerId,
                Type = type,
                AsOfDate = reportDate,
                TotalBilled = details.Sum(d => d.TotalAmount),
                TotalPaid = details.Sum(d => d.PaidAmount),
                TotalOutstanding = details.Sum(d => d.PendingAmount),
                Invoices = details
            };
        }
    }
}
