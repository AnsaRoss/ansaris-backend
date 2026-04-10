using ErpApp.Application.Constants;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ErpApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Application.UseCases.Invoice
{
    public class GenerateAccountingEntriesUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAccountTransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GenerateAccountingEntriesUseCase(
            IInvoiceRepository invoiceRepository,
            IAccountTransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice), "Invoice not found");

            // Define las cuentas contables según el tipo de factura
            Guid debitAccountId;
            Guid creditAccountId;

            if (invoice.Type == InvoiceType.Sale)
            {
                debitAccountId = LedgerAccounts.AccountsReceivable; // Cliente debe dinero
                creditAccountId = LedgerAccounts.SalesRevenue;      // Ingreso por ventas
            }
            else // Purchase
            {
                debitAccountId = LedgerAccounts.Inventory;           // Compras (Inventario)
                creditAccountId = LedgerAccounts.AccountsPayable;    // Proveedores deben dinero
            }

            // Crear la transacción contable
            var transaction = new AccountTransaction
            {
                //Id = Guid.NewGuid(),
                Date = invoice.Date,
                DebitAccountId = debitAccountId,
                CreditAccountId = creditAccountId,
                Amount = invoice.TotalAmount,
                Description = $"Contabilización de factura #{invoice.InvoiceNumber}",
                InvoiceId = invoice.Id
            };

            await _transactionRepository.AddAsync(transaction);

            await _unitOfWork.CommitAsync();
        }
    }
}
