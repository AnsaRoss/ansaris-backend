using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class GetAllInvoicesUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetAllInvoicesUseCase(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IEnumerable<global::ErpApp.Domain.Entities.Invoice>> ExecuteAsync()
        {
            return await _invoiceRepository.GetAllAsync();
        }
    }
}
