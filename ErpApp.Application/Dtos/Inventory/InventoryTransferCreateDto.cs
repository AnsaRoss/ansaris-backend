namespace ErpApp.Application.Dtos.Inventory
{
    public class InventoryTransferCreateDto
    {
        public int SourceWarehouseId { get; set; }
        public int DestinationWarehouseId { get; set; }
        public string? Notes { get; set; }
        public string? PerformedBy { get; set; }
        public List<InventoryTransferItemDto> Items { get; set; } = new();
    }
}
