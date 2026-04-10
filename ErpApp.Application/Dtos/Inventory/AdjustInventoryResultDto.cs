namespace ErpApp.Application.Dtos.Inventory
{
    public class AdjustInventoryResultDto
    {
        public int MovementId { get; set; }
        public int ProductId { get; set; }
        public int QuantityDelta { get; set; }
        public int StockBefore { get; set; }
        public int StockAfter { get; set; }
        public DateTime MovementDate { get; set; }
    }
}
