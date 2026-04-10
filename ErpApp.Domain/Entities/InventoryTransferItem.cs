namespace ErpApp.Domain.Entities
{
    public class InventoryTransferItem
    {
        public int Id { get; set; }
        public int InventoryTransferId { get; set; }
        public InventoryTransfer InventoryTransfer { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}
