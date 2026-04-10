using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class GetInventoryStockUseCase
    {
        private readonly IProductRepository _productRepository;

        public GetInventoryStockUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<InventoryStockDto>> ExecuteAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return products
                .OrderBy(p => p.Name)
                .Select(p => new InventoryStockDto
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Stock = p.Stock,
                    ReservedStock = p.ReservedStock,
                    AvailableStock = p.Stock - p.ReservedStock
                })
                .ToList();
        }
    }
}
