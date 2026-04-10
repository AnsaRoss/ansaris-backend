namespace ErpApp.Application.Dtos.Inventory
{
    public class InventoryValuationDto
    {
        public int ProductId { get; set; }
        public string Method { get; set; } = string.Empty;
        public int QuantityOnHand { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue { get; set; }
    }
}
