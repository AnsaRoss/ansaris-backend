namespace ErpApp.Domain.Entities
{
    public class InventoryMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int? WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public DateTime MovementDate { get; set; } = DateTime.UtcNow;
        public InventoryMovementType MovementType { get; set; }
        public int Quantity { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public int ReservedQuantityDelta { get; set; }
        public int ReservedBefore { get; set; }
        public int ReservedAfter { get; set; }
        public decimal UnitCost { get; set; }
        public string ReferenceType { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsReversed { get; set; }
        public int? ReversalOfMovementId { get; set; }
    }
}
