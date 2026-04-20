using ErpApp.Application.Constants;
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
        private readonly ITreasuryAccountRepository _treasuryAccountRepository;
        private readonly ITreasuryMovementRepository _treasuryMovementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterPaymentUseCase(
            IInvoiceRepository invoiceRepository,
            IAccountTransactionRepository transactionRepository,
            ITreasuryAccountRepository treasuryAccountRepository,
            ITreasuryMovementRepository treasuryMovementRepository,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _transactionRepository = transactionRepository;
            _treasuryAccountRepository = treasuryAccountRepository;
            _treasuryMovementRepository = treasuryMovementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(int invoiceId, int treasuryAccountId, decimal paymentAmount, DateTime paymentDate, string? notes)
        {
            if (treasuryAccountId <= 0)
                throw new Exception("Debe indicar una cuenta de tesorería válida.");

            if (paymentAmount <= 0)
                throw new Exception("El monto del pago debe ser mayor que cero.");

            // Obtener la factura
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
                throw new Exception($"Factura con ID {invoiceId} no encontrada.");

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new Exception("No se puede registrar pago en una factura anulada.");

            if (invoice.Status == InvoiceStatus.Paid)
                throw new Exception("La factura ya se encuentra totalmente pagada.");

            var pendingAmount = invoice.TotalAmount - invoice.PaidAmount;
            if (paymentAmount > pendingAmount)
                throw new Exception($"El pago ({paymentAmount}) excede el saldo pendiente ({pendingAmount}).");

            var treasuryAccount = await _treasuryAccountRepository.GetByIdAsync(treasuryAccountId);
            if (treasuryAccount == null)
                throw new Exception($"Cuenta de tesorería con ID {treasuryAccountId} no encontrada.");

            if (!treasuryAccount.IsActive)
                throw new Exception("La cuenta de tesorería está inactiva.");

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

            var treasuryBalanceBefore = treasuryAccount.Balance;
            if (invoice.Type == InvoiceType.Sale)
            {
                treasuryAccount.Balance += paymentAmount;
            }
            else
            {
                if (treasuryAccount.Balance < paymentAmount)
                    throw new Exception("Fondos insuficientes en la cuenta de tesorería para registrar el pago.");

                treasuryAccount.Balance -= paymentAmount;
            }

            var treasuryMovement = new TreasuryMovement
            {
                TreasuryAccountId = treasuryAccount.Id,
                MovementDate = paymentDate,
                Type = invoice.Type == InvoiceType.Sale ? TreasuryMovementType.Inflow : TreasuryMovementType.Outflow,
                Amount = paymentAmount,
                BalanceBefore = treasuryBalanceBefore,
                BalanceAfter = treasuryAccount.Balance,
                ReferenceType = "InvoicePayment",
                ReferenceNumber = invoice.InvoiceNumber ?? invoice.Id.ToString(),
                Notes = notes
            };

            await _treasuryMovementRepository.AddAsync(treasuryMovement);

            // Commit de todo
            await _unitOfWork.CommitAsync();
        }
    }
}