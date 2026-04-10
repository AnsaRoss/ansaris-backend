namespace ErpApp.Application.Dtos.Inventory
{
    public class InventoryTransferResultDto
    {
        public int TransferId { get; set; }
        public string TransferNumber { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public int SourceWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public int ItemsCount { get; set; }
    }
}
