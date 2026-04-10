using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class GetWarehouseStockUseCase
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IProductWarehouseStockRepository _warehouseStockRepository;

        public GetWarehouseStockUseCase(
            IWarehouseRepository warehouseRepository,
            IProductWarehouseStockRepository warehouseStockRepository)
        {
            _warehouseRepository = warehouseRepository;
            _warehouseStockRepository = warehouseStockRepository;
        }

        public async Task<List<WarehouseStockDto>> ExecuteAsync(int warehouseId)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId)
                ?? throw new Exception("Almacén no encontrado.");

            var stocks = await _warehouseStockRepository.GetByWarehouseAsync(warehouse.Id);

            return stocks.Select(s => new WarehouseStockDto
            {
                WarehouseId = warehouse.Id,
                WarehouseCode = warehouse.Code,
                WarehouseName = warehouse.Name,
                ProductId = s.ProductId,
                ProductName = s.Product.Name,
                SKU = s.Product.SKU,
                OnHand = s.OnHand,
                Reserved = s.Reserved,
                Available = s.OnHand - s.Reserved
            }).ToList();
        }
    }
}
