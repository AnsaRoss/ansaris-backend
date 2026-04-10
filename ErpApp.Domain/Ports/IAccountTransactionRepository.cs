using ErpApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Ports
{
    public interface IAccountTransactionRepository
    {
        Task AddAsync(AccountTransaction transaction);
    }
}
