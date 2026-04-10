using ErpApp.Domain;

namespace ErpApp.Domain.Entities
{
    public class InventoryTransfer
    {
        public int Id { get; set; }
        public int SequenceNumber { get; set; }
        public string TransferNumber { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
        public int SourceWarehouseId { get; set; }
        public Warehouse SourceWarehouse { get; set; } = null!;
        public int DestinationWarehouseId { get; set; }
        public Warehouse DestinationWarehouse { get; set; } = null!;
        public InventoryTransferStatus Status { get; set; } = InventoryTransferStatus.Completed;
        public string? Notes { get; set; }
        public List<InventoryTransferItem> Items { get; set; } = new();
    }
}
