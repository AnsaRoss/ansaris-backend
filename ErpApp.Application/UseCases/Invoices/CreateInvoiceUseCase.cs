using ErpApp.Domain.Entities;
using ErpApp.Domain;
using ErpApp.Domain.Entities;
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
            if (dto.Items == null || dto.Items.Count == 0)
                throw new Exception("La factura debe tener al menos un ítem.");

            if (dto.TaxRate < 0 || dto.TaxRate > 100)
                throw new Exception("La tasa de IVA debe estar entre 0 y 100.");

            var invoiceItems = new List<InvoiceItem>();
            decimal subtotalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Producto con ID {item.ProductId} no encontrado.");

                if (item.Quantity <= 0)
                    throw new Exception($"Cantidad inválida para el producto {item.ProductId}.");
                
                var itemTotal = product.Price * item.Quantity;
                subtotalAmount += itemTotal;

                var invoiceItem = new InvoiceItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                invoiceItems.Add(invoiceItem);
            }

            if (dto.DiscountAmount < 0)
                throw new Exception("El descuento no puede ser negativo.");

            if (dto.DiscountAmount > subtotalAmount)
                throw new Exception("El descuento no puede ser mayor al subtotal.");

            var taxableBase = subtotalAmount - dto.DiscountAmount;
            var taxAmount = Math.Round(taxableBase * (dto.TaxRate / 100m), 2);
            var totalAmount = taxableBase + taxAmount;

            var invoiceDate = DateTime.Now;
            var series = string.IsNullOrWhiteSpace(dto.Series) ? "A" : dto.Series.Trim().ToUpperInvariant();
            var sequenceNumber = await _invoiceRepository.GetNextSequenceNumberAsync(series, invoiceDate);

            var invoice = new Domain.Entities.Invoice
            {
                Date = invoiceDate,
                Type = dto.Type,
                CustomerId = dto.CustomerId,
                Series = series,
                SequenceNumber = sequenceNumber,
                InvoiceNumber = $"{series}-{invoiceDate:yyyyMMdd}-{sequenceNumber:D6}",
                SubtotalAmount = subtotalAmount,
                DiscountAmount = dto.DiscountAmount,
                TaxRate = dto.TaxRate,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                PaidAmount = 0,
                Status = InvoiceStatus.Pending,
                Items = invoiceItems
            };

            await _invoiceRepository.AddAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();

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

