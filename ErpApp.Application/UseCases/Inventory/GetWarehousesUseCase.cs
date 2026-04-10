using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class GetWarehousesUseCase
    {
        private readonly IWarehouseRepository _warehouseRepository;

        public GetWarehousesUseCase(IWarehouseRepository warehouseRepository)
        {
            _warehouseRepository = warehouseRepository;
        }

        public async Task<List<WarehouseDto>> ExecuteAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();

            return warehouses.Select(w => new WarehouseDto
            {
                Id = w.Id,
                Code = w.Code,
                Name = w.Name,
                IsActive = w.IsActive
            }).ToList();
        }
    }
}
