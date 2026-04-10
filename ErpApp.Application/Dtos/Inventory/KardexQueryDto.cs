using ErpApp.Domain;

namespace ErpApp.Application.Dtos.Inventory
{
    public class KardexQueryDto
    {
        public int? ProductId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public InventoryMovementType? MovementType { get; set; }
        public string? ReferenceType { get; set; }
        public string? ReferenceNumber { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
