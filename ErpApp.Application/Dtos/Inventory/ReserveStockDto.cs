namespace ErpApp.Application.Dtos.Inventory
{
    public class ReserveStockDto
    {
        public int? WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ReferenceType { get; set; } = "SalesOrder";
        public string ReferenceNumber { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? PerformedBy { get; set; }
    }
}
