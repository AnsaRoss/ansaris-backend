using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class GetInvoiceByIdUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoiceByIdUseCase(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<global::ErpApp.Domain.Entities.Invoice?> ExecuteAsync(int id)
        {
            return await _invoiceRepository.GetByIdAsync(id);
        }
    }
}
