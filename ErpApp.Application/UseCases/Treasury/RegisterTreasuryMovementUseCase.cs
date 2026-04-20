using ErpApp.Application.Dtos.Treasury;
using ErpApp.Domain;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Treasury
{
    public class RegisterTreasuryMovementUseCase
    {
        private readonly ITreasuryAccountRepository _accountRepository;
        private readonly ITreasuryMovementRepository _movementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterTreasuryMovementUseCase(
            ITreasuryAccountRepository accountRepository,
            ITreasuryMovementRepository movementRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _movementRepository = movementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TreasuryMovementDto> ExecuteAsync(RegisterTreasuryMovementDto dto)
        {
            if (dto.Amount <= 0)
                throw new Exception("El monto debe ser mayor que cero.");

            if (dto.Type == TreasuryMovementType.Transfer)
                throw new Exception("Para transferencias usa el endpoint de transferencias.");

            var account = await _accountRepository.GetByIdAsync(dto.TreasuryAccountId)
                ?? throw new Exception("Cuenta de tesorería no encontrada.");

            if (!account.IsActive)
                throw new Exception("La cuenta está inactiva.");

            var balanceBefore = account.Balance;
            var balanceAfter = dto.Type == TreasuryMovementType.Inflow
                ? balanceBefore + dto.Amount
                : balanceBefore - dto.Amount;

            if (balanceAfter < 0)
                throw new Exception("Fondos insuficientes para registrar el egreso.");

            account.Balance = balanceAfter;

            var movement = new TreasuryMovement
            {
                TreasuryAccountId = account.Id,
                MovementDate = DateTime.UtcNow,
                Type = dto.Type,
                Amount = dto.Amount,
                BalanceBefore = balanceBefore,
                BalanceAfter = balanceAfter,
                ReferenceType = string.IsNullOrWhiteSpace(dto.ReferenceType) ? "Manual" : dto.ReferenceType,
                ReferenceNumber = string.IsNullOrWhiteSpace(dto.ReferenceNumber) ? "MANUAL" : dto.ReferenceNumber,
                Notes = dto.Notes
            };

            await _movementRepository.AddAsync(movement);
            await _unitOfWork.CommitAsync();

            return new TreasuryMovementDto
            {
                Id = movement.Id,
                MovementDate = movement.MovementDate,
                TreasuryAccountId = movement.TreasuryAccountId,
                TreasuryAccountCode = account.Code,
                Type = movement.Type,
                Amount = movement.Amount,
                BalanceBefore = movement.BalanceBefore,
                BalanceAfter = movement.BalanceAfter,
                ReferenceType = movement.ReferenceType,
                ReferenceNumber = movement.ReferenceNumber,
                Notes = movement.Notes
            };
        }
    }
}
