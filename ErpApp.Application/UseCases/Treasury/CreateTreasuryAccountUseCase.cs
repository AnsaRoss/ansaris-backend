using ErpApp.Application.Dtos.Treasury;
using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Treasury
{
    public class CreateTreasuryAccountUseCase
    {
        private readonly ITreasuryAccountRepository _accountRepository;
        private readonly ITreasuryMovementRepository _movementRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTreasuryAccountUseCase(
            ITreasuryAccountRepository accountRepository,
            ITreasuryMovementRepository movementRepository,
            IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _movementRepository = movementRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TreasuryAccountDto> ExecuteAsync(TreasuryAccountCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new Exception("El código es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("El nombre es obligatorio.");

            if (dto.OpeningBalance < 0)
                throw new Exception("El saldo inicial no puede ser negativo.");

            var code = dto.Code.Trim().ToUpperInvariant();
            var existing = await _accountRepository.GetByCodeAsync(code);
            if (existing != null)
                throw new Exception($"Ya existe una cuenta con código {code}.");

            var account = new TreasuryAccount
            {
                Code = code,
                Name = dto.Name.Trim(),
                Type = dto.Type,
                Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "USD" : dto.Currency.Trim().ToUpperInvariant(),
                Balance = dto.OpeningBalance,
                IsActive = true
            };

            await _accountRepository.AddAsync(account);

            if (dto.OpeningBalance > 0)
            {
                await _movementRepository.AddAsync(new TreasuryMovement
                {
                    TreasuryAccount = account,
                    MovementDate = DateTime.UtcNow,
                    Type = Domain.TreasuryMovementType.Inflow,
                    Amount = dto.OpeningBalance,
                    BalanceBefore = 0,
                    BalanceAfter = dto.OpeningBalance,
                    ReferenceType = "OpeningBalance",
                    ReferenceNumber = "OPENING",
                    Notes = "Saldo inicial"
                });
            }

            await _unitOfWork.CommitAsync();

            return new TreasuryAccountDto
            {
                Id = account.Id,
                Code = account.Code,
                Name = account.Name,
                Type = account.Type,
                Currency = account.Currency,
                Balance = account.Balance,
                IsActive = account.IsActive
            };
        }
    }
}
