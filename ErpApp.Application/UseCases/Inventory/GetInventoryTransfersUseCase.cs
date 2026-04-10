using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class GetInventoryTransfersUseCase
    {
        private readonly IInventoryTransferRepository _transferRepository;

        public GetInventoryTransfersUseCase(IInventoryTransferRepository transferRepository)
        {
            _transferRepository = transferRepository;
        }

        public async Task<List<InventoryTransferResultDto>> ExecuteAsync()
        {
            var transfers = await _transferRepository.GetAllAsync();
            return transfers.Select(t => new InventoryTransferResultDto
            {
                TransferId = t.Id,
                TransferNumber = t.TransferNumber,
                TransferDate = t.TransferDate,
                SourceWarehouseId = t.SourceWarehouseId,
                DestinationWarehouseId = t.DestinationWarehouseId,
                ItemsCount = t.Items.Count
            }).ToList();
        }
    }
}
