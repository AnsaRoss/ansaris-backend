using ErpApp.Application.Dtos.Treasury;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Treasury
{
    public class GetTreasuryMovementsUseCase
    {
        private readonly ITreasuryMovementRepository _movementRepository;

        public GetTreasuryMovementsUseCase(ITreasuryMovementRepository movementRepository)
        {
            _movementRepository = movementRepository;
        }

        public async Task<List<TreasuryMovementDto>> ExecuteAsync(int? treasuryAccountId, DateTime? from, DateTime? to)
        {
            var movements = await _movementRepository.GetAsync(treasuryAccountId, from, to);

            return movements.Select(m => new TreasuryMovementDto
            {
                Id = m.Id,
                MovementDate = m.MovementDate,
                TreasuryAccountId = m.TreasuryAccountId,
                TreasuryAccountCode = m.TreasuryAccount.Code,
                Type = m.Type,
                Amount = m.Amount,
                BalanceBefore = m.BalanceBefore,
                BalanceAfter = m.BalanceAfter,
                ReferenceType = m.ReferenceType,
                ReferenceNumber = m.ReferenceNumber,
                Notes = m.Notes
            }).ToList();
        }
    }
}
