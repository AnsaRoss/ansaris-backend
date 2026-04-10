namespace ErpApp.Application.Dtos.Inventory
{
    public class KardexPagedResultDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<KardexMovementDto> Items { get; set; } = new();
    }
}
