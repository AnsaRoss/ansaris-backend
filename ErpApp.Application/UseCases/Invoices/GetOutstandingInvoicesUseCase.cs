using ErpApp.Application.Dtos.Invoice;
using ErpApp.Application.Dtos.Invoice;
using ErpApp.Domain;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class GetOutstandingInvoicesUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetOutstandingInvoicesUseCase(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<List<OutstandingInvoiceDto>> ExecuteAsync(InvoiceType? type)
        {
            var invoices = await _invoiceRepository.GetAllAsync();

            var query = invoices
                .Where(i => i.Status != InvoiceStatus.Cancelled)
                .Where(i => i.TotalAmount > i.PaidAmount);

            if (type.HasValue)
            {
                query = query.Where(i => i.Type == type.Value);
            }

            var today = DateTime.UtcNow.Date;

            return query
                .OrderBy(i => i.Date)
                .Select(i => new OutstandingInvoiceDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber ?? i.Id.ToString(),
                    CustomerId = i.CustomerId,
                    WarehouseId = i.WarehouseId,
                    Date = i.Date,
                    DueDate = i.DueDate == default ? i.Date.Date : i.DueDate.Date,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    PendingAmount = i.TotalAmount - i.PaidAmount,
                    DaysOpen = Math.Max((today - i.Date.Date).Days, 0),
                    DaysPastDue = Math.Max((today - (i.DueDate == default ? i.Date.Date : i.DueDate.Date)).Days, 0)
                })
                .ToList();
        }
    }
}
