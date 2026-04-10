using ErpApp.Application.Dtos.Inventory;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Inventory
{
    public class GetKardexUseCase
    {
        private readonly IInventoryMovementRepository _movementRepository;

        public GetKardexUseCase(IInventoryMovementRepository movementRepository)
        {
            _movementRepository = movementRepository;
        }

        public async Task<KardexPagedResultDto> ExecuteAsync(KardexQueryDto dto)
        {
            var pageNumber = dto.PageNumber <= 0 ? 1 : dto.PageNumber;
            var pageSize = dto.PageSize <= 0 ? 20 : Math.Min(dto.PageSize, 200);

            var (items, totalCount) = await _movementRepository.SearchAsync(
                dto.ProductId,
                dto.From,
                dto.To,
                dto.MovementType,
                dto.ReferenceType,
                dto.ReferenceNumber,
                pageNumber,
                pageSize);

            return new KardexPagedResultDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items.Select(m => new KardexMovementDto
                {
                    Id = m.Id,
                    ProductId = m.ProductId,
                    MovementDate = m.MovementDate,
                    MovementType = m.MovementType,
                    Quantity = m.Quantity,
                    StockBefore = m.StockBefore,
                    StockAfter = m.StockAfter,
                    ReservedBefore = m.ReservedBefore,
                    ReservedAfter = m.ReservedAfter,
                    UnitCost = m.UnitCost,
                    ReferenceType = m.ReferenceType,
                    ReferenceNumber = m.ReferenceNumber,
                    IsReversed = m.IsReversed,
                    ReversalOfMovementId = m.ReversalOfMovementId,
                    Notes = m.Notes
                }).ToList()
            };
        }
    }
}
