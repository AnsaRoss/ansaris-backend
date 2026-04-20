using ErpApp.Application.Dtos.Treasury;
using ErpApp.Domain.Ports;

namespace ErpApp.Application.UseCases.Treasury
{
    public class GetTreasuryAccountsUseCase
    {
        private readonly ITreasuryAccountRepository _accountRepository;

        public GetTreasuryAccountsUseCase(ITreasuryAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<TreasuryAccountDto>> ExecuteAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();

            return accounts.Select(a => new TreasuryAccountDto
            {
                Id = a.Id,
                Code = a.Code,
                Name = a.Name,
                Type = a.Type,
                Currency = a.Currency,
                Balance = a.Balance,
                IsActive = a.IsActive
            }).ToList();
        }
    }
}
