using ErpApp.Domain;

namespace ErpApp.Application.Dtos.Inventory
{
    public class KardexMovementDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public DateTime MovementDate { get; set; }
        public InventoryMovementType MovementType { get; set; }
        public int Quantity { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public int ReservedBefore { get; set; }
        public int ReservedAfter { get; set; }
        public decimal UnitCost { get; set; }
        public string ReferenceType { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public bool IsReversed { get; set; }
        public int? ReversalOfMovementId { get; set; }
        public string? Notes { get; set; }
    }
}
