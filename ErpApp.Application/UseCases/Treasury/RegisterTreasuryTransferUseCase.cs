using ErpApp.Application.Dtos.Treasury;
using ErpApp.Domain;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Treasury
{
    public class RegisterTreasuryTransferUseCase
    {
        private readonly ITreasuryAccountRepository _accountRepository;
        private readonly ITreasuryMovementRepository _movementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterTreasuryTransferUseCase(
            ITreasuryAccountRepository accountRepository,
            ITreasuryMovementRepository movementRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _movementRepository = movementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(TreasuryTransferDto dto)
        {
            if (dto.FromTreasuryAccountId == dto.ToTreasuryAccountId)
                throw new Exception("Las cuentas de origen y destino no pueden ser iguales.");

            if (dto.Amount <= 0)
                throw new Exception("El monto debe ser mayor que cero.");

            var from = await _accountRepository.GetByIdAsync(dto.FromTreasuryAccountId)
                ?? throw new Exception("Cuenta origen no encontrada.");

            var to = await _accountRepository.GetByIdAsync(dto.ToTreasuryAccountId)
                ?? throw new Exception("Cuenta destino no encontrada.");

            if (!from.IsActive || !to.IsActive)
                throw new Exception("Ambas cuentas deben estar activas.");

            if (from.Currency != to.Currency)
                throw new Exception("La transferencia entre monedas distintas no está soportada en esta fase.");

            var fromBefore = from.Balance;
            var toBefore = to.Balance;

            if (fromBefore < dto.Amount)
                throw new Exception("Fondos insuficientes en cuenta origen.");

            from.Balance -= dto.Amount;
            to.Balance += dto.Amount;

            var referenceNumber = string.IsNullOrWhiteSpace(dto.ReferenceNumber)
                ? $"TR-{DateTime.UtcNow:yyyyMMddHHmmss}"
                : dto.ReferenceNumber.Trim();

            await _movementRepository.AddAsync(new TreasuryMovement
            {
                TreasuryAccountId = from.Id,
                CounterpartyTreasuryAccountId = to.Id,
                MovementDate = DateTime.UtcNow,
                Type = TreasuryMovementType.Transfer,
                Amount = dto.Amount,
                BalanceBefore = fromBefore,
                BalanceAfter = from.Balance,
                ReferenceType = "TransferOut",
                ReferenceNumber = referenceNumber,
                Notes = dto.Notes
            });

            await _movementRepository.AddAsync(new TreasuryMovement
            {
                TreasuryAccountId = to.Id,
                CounterpartyTreasuryAccountId = from.Id,
                MovementDate = DateTime.UtcNow,
                Type = TreasuryMovementType.Transfer,
                Amount = dto.Amount,
                BalanceBefore = toBefore,
                BalanceAfter = to.Balance,
                ReferenceType = "TransferIn",
                ReferenceNumber = referenceNumber,
                Notes = dto.Notes
            });

            await _unitOfWork.CommitAsync();
        }
    }
}
