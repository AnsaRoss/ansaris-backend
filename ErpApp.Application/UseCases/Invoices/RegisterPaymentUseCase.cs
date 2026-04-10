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
    public class RegisterPaymentUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAccountTransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterPaymentUseCase(
            IInvoiceRepository invoiceRepository,
            IAccountTransactionRepository transactionRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int invoiceId, decimal paymentAmount, DateTime paymentDate)
        {
            // Obtener la factura
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
                throw new Exception($"Factura con ID {invoiceId} no encontrada.");

            // Actualizar monto pagado
            invoice.PaidAmount += paymentAmount;

            // Actualizar estado de factura
            if (invoice.PaidAmount >= invoice.TotalAmount)
                invoice.Status = InvoiceStatus.Paid;
            else
                invoice.Status = InvoiceStatus.Pending;

            await _invoiceRepository.UpdateAsync(invoice);

            // Crear la transacción contable para el pago
            Guid debitAccountId;
            Guid creditAccountId;

            if (invoice.Type == InvoiceType.Sale)
            {
                debitAccountId = LedgerAccounts.CashOrBank;          // Caja o Banco (aumenta activo)
                creditAccountId = LedgerAccounts.AccountsReceivable; // Disminuye cuentas por cobrar
            }
            else // Purchase
            {
                debitAccountId = LedgerAccounts.AccountsPayable;    // Disminuye pasivo
                creditAccountId = LedgerAccounts.CashOrBank;        // Caja o Banco (disminuye activo)
            }

            var paymentTransaction = new AccountTransaction
            {
                //Id = Guid.NewGuid(),
                Date = paymentDate,
                DebitAccountId = debitAccountId,
                CreditAccountId = creditAccountId,
                Amount = paymentAmount,
                Description = $"Pago registrado para factura #{invoice.InvoiceNumber}",
                InvoiceId = invoice.Id
            };

            await _transactionRepository.AddAsync(paymentTransaction);

            // Commit de todo
            await _unitOfWork.CommitAsync();
        }
    }
}