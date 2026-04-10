using ErpApp.Application.Constants;
using ErpApp.Domain;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Invoices
{
    public class CancelInvoiceUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAccountTransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelInvoiceUseCase(
            IInvoiceRepository invoiceRepository,
            IAccountTransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int invoiceId, string? reason)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
                throw new Exception($"Factura con ID {invoiceId} no encontrada.");

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new Exception("La factura ya se encuentra anulada.");

            if (invoice.PaidAmount > 0)
                throw new Exception("No se puede anular una factura con pagos registrados.");

            invoice.Status = InvoiceStatus.Cancelled;
            await _invoiceRepository.UpdateAsync(invoice);

            Guid debitAccountId;
            Guid creditAccountId;

            if (invoice.Type == InvoiceType.Sale)
            {
                debitAccountId = LedgerAccounts.SalesRevenue;
                creditAccountId = LedgerAccounts.AccountsReceivable;
            }
            else
            {
                debitAccountId = LedgerAccounts.AccountsPayable;
                creditAccountId = LedgerAccounts.Inventory;
            }

            var cancellationTransaction = new AccountTransaction
            {
                Date = DateTime.Now,
                DebitAccountId = debitAccountId,
                CreditAccountId = creditAccountId,
                Amount = invoice.TotalAmount,
                Description = $"Anulación de factura #{invoice.InvoiceNumber}. Motivo: {reason ?? "No especificado"}",
                InvoiceId = invoice.Id
            };

            await _transactionRepository.AddAsync(cancellationTransaction);
            await _unitOfWork.CommitAsync();
        }
    }
}
