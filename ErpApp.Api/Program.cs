using ErpApp.Application.UseCases.Customers;
using ErpApp.Application.UseCases.Invoice;
using ErpApp.Application.UseCases.Invoices;
using ErpApp.Application.UseCases.Inventory;
using ErpApp.Application.UseCases.Products;
using ErpApp.Application.UseCases.PurchaseOrders;
using ErpApp.Application.UseCases.SaleOrders;
using ErpApp.Application.UseCases.Treasury;
using ErpApp.Domain.Ports;
using ErpApp.Persistence.Repositories;
using ERPApp.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Configuración de la base de datos
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🧩 Inyección de dependencias
// Repositorios
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IAccountTransactionRepository, AccountTransactionRepository>();
builder.Services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IProductWarehouseStockRepository, ProductWarehouseStockRepository>();
builder.Services.AddScoped<IInventoryTransferRepository, InventoryTransferRepository>();
builder.Services.AddScoped<ITreasuryAccountRepository, TreasuryAccountRepository>();
builder.Services.AddScoped<ITreasuryMovementRepository, TreasuryMovementRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// UseCases Products
builder.Services.AddScoped<IGetAllProductsUseCase, GetAllProductsUseCase>();
builder.Services.AddScoped<ICreateProductUseCase, CreateProductUseCase>();
builder.Services.AddScoped<IGetProductByIdUseCase, GetProductByIdUseCase>();


// UseCases PurchaseOrders
builder.Services.AddScoped<CreatePurchaseOrderUseCase>();
builder.Services.AddScoped<GetAllPurchaseOrdersUseCase>();
builder.Services.AddScoped<GetPurchaseOrderByIdUseCase>();
builder.Services.AddScoped<ReceivePurchaseOrderUseCase>();
builder.Services.AddScoped<CancelPurchaseOrderUseCase>();

// UseCases SalesOrder
builder.Services.AddScoped<CreateSalesOrderUseCase>();
builder.Services.AddScoped<UpdateSalesOrderStatusUseCase>();
builder.Services.AddScoped<GetAllSalesOrdersUseCase>();
builder.Services.AddScoped<GetSalesOrderByIdUseCase>();

// UseCases Customer
builder.Services.AddScoped<CreateCustomerUseCase>();
builder.Services.AddScoped<UpdateCustomerUseCase>();
builder.Services.AddScoped<GetCustomerByIdUseCase>();
builder.Services.AddScoped<GetAllCustomersUseCase>();
builder.Services.AddScoped<DeleteCustomerUseCase>();

//Use
builder.Services.AddScoped<CreateInvoiceUseCase>();
builder.Services.AddScoped<GenerateAccountingEntriesUseCase>();
builder.Services.AddScoped<RegisterPaymentUseCase>();
builder.Services.AddScoped<CancelInvoiceUseCase>();
builder.Services.AddScoped<GetInvoiceByIdUseCase>();
builder.Services.AddScoped<GetAllInvoicesUseCase>();
builder.Services.AddScoped<GetInvoicePaymentsUseCase>();
builder.Services.AddScoped<GetOutstandingInvoicesUseCase>();
builder.Services.AddScoped<GetInvoiceAgingReportUseCase>();
builder.Services.AddScoped<GetAgingByCustomerUseCase>();
builder.Services.AddScoped<GetCustomerStatementUseCase>();
builder.Services.AddScoped<GetInvoiceKpiSummaryUseCase>();

// UseCases Inventory
builder.Services.AddScoped<GetInventoryStockUseCase>();
builder.Services.AddScoped<GetInventoryMovementsByProductUseCase>();
builder.Services.AddScoped<GetKardexUseCase>();
builder.Services.AddScoped<GetInventoryValuationUseCase>();
builder.Services.AddScoped<AdjustInventoryUseCase>();
builder.Services.AddScoped<ReserveStockUseCase>();
builder.Services.AddScoped<ReleaseStockUseCase>();
builder.Services.AddScoped<ReverseInventoryMovementUseCase>();
builder.Services.AddScoped<CreateWarehouseUseCase>();
builder.Services.AddScoped<GetWarehousesUseCase>();
builder.Services.AddScoped<GetWarehouseStockUseCase>();
builder.Services.AddScoped<ExecuteInventoryTransferUseCase>();
builder.Services.AddScoped<GetInventoryTransfersUseCase>();

// UseCases Treasury
builder.Services.AddScoped<CreateTreasuryAccountUseCase>();
builder.Services.AddScoped<GetTreasuryAccountsUseCase>();
builder.Services.AddScoped<RegisterTreasuryMovementUseCase>();
builder.Services.AddScoped<RegisterTreasuryTransferUseCase>();
builder.Services.AddScoped<GetTreasuryMovementsUseCase>();


// 📦 Servicios de controladores + Swagger
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// 🌐 Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 🗺️ Mapea los controladores
app.MapControllers();

app.Run();

