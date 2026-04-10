using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ERPApp.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Persistence.Repositories
{
    public class AccountTransactionRepository : IAccountTransactionRepository
    {
        private readonly AppDbContext _context;

        public AccountTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AccountTransaction transaction)
        {
            await _context.AccountTransactions.AddAsync(transaction);
        }
    }
}
