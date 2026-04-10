using ErpApp.Domain.Entities;
using ErpApp.Domain;
using ErpApp.Domain.Ports;
using ErpApp.Application.Dtos.Invoice;
using ErpApp.Application.Constants;

namespace ErpApp.Application.UseCases.Invoices
{
    public class CreateInvoiceUseCase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IAccountTransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepository;

        public CreateInvoiceUseCase(
            IInvoiceRepository invoiceRepository,
            IAccountTransactionRepository transactionRepository,
            IUnitOfWork unitOfWork,
            IProductRepository productRepository)
        {
            _invoiceRepository = invoiceRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public async Task<int> ExecuteAsync(CreateInvoiceDto dto)
        {
            var invoiceItems = new List<InvoiceItem>();
            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Producto con ID {item.ProductId} no encontrado.");
                
                var itemTotal = product.Price * item.Quantity;
                totalAmount += itemTotal;

                var invoiceItem = new InvoiceItem
                {
                    //Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                totalAmount += product.Price * item.Quantity;
                invoiceItems.Add(invoiceItem);
            }
            // 1. Crear la factura
            var invoice = new Domain.Entities.Invoice
            {
                
                Date = DateTime.Now,
                Type = dto.Type,
                CustomerId = dto.CustomerId,
                TotalAmount = totalAmount,
                PaidAmount = 0,
                Status = InvoiceStatus.Pending,
                Items = invoiceItems
            };

            await _invoiceRepository.AddAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();
            invoice.InvoiceNumber = $"INV-{invoice.Date:yyyyMMdd}-{invoice.Id.ToString().Substring(0, 8)}";
            await _invoiceRepository.UpdateAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();

            // 2. Crear la transacción contable básica
            var debitAccountId = dto.Type == InvoiceType.Sale
                ? LedgerAccounts.AccountsReceivable
                : LedgerAccounts.Inventory;

            var creditAccountId = dto.Type == InvoiceType.Sale
                ? LedgerAccounts.SalesRevenue
                : LedgerAccounts.AccountsPayable;

            var transaction = new AccountTransaction
            {
                //Id = Guid.NewGuid(),
                Date = DateTime.Now,
                DebitAccountId = debitAccountId,
                CreditAccountId = creditAccountId,
                Amount = totalAmount,
                Description = $"Generada factura #{invoice.InvoiceNumber}",
                InvoiceId = invoice.Id
            };

            await _transactionRepository.AddAsync(transaction);

            await _unitOfWork.CommitAsync();

            return invoice.Id;
        }
    }
}

