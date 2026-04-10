namespace ErpApp.Domain.Entities
{
    public class ProductWarehouseStock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
        public int OnHand { get; set; }
        public int Reserved { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
