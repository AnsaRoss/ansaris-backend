using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class CreateWarehouseUseCase
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateWarehouseUseCase(IWarehouseRepository warehouseRepository, IUnitOfWork unitOfWork)
        {
            _warehouseRepository = warehouseRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<WarehouseDto> ExecuteAsync(WarehouseCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new Exception("El código del almacén es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("El nombre del almacén es obligatorio.");

            var normalizedCode = dto.Code.Trim().ToUpperInvariant();
            var existing = await _warehouseRepository.GetByCodeAsync(normalizedCode);
            if (existing != null)
                throw new Exception($"Ya existe un almacén con código {normalizedCode}.");

            var warehouse = new Warehouse
            {
                Code = normalizedCode,
                Name = dto.Name.Trim(),
                IsActive = true
            };

            await _warehouseRepository.AddAsync(warehouse);
            await _unitOfWork.CommitAsync();

            return new WarehouseDto
            {
                Id = warehouse.Id,
                Code = warehouse.Code,
                Name = warehouse.Name,
                IsActive = warehouse.IsActive
            };
        }
    }
}
