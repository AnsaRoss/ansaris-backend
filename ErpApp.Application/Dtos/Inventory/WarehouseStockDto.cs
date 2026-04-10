namespace ErpApp.Application.Dtos.Inventory
{
    public class WarehouseStockDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int OnHand { get; set; }
        public int Reserved { get; set; }
        public int Available { get; set; }
    }
}
