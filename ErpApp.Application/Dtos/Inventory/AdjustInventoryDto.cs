namespace ErpApp.Application.Dtos.Inventory
{
    public class AdjustInventoryDto
    {
        public int? WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int QuantityDelta { get; set; }
        public decimal UnitCost { get; set; }
        public string? Notes { get; set; }
        public string? PerformedBy { get; set; }
    }
}
