using ErpApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Ports
{
    public interface ISalesOrderRepository
    {
        Task AddAsync(SalesOrder order);
        Task<SalesOrder?> GetByIdAsync(string orderNumber);
        Task<List<SalesOrder>> GetAllAsync();
        Task SaveChangesAsync();
        Task UpdateAsync(SalesOrder order);
        Task<SalesOrder> GetByOrderNumberAsync(string orderNumber);
    }

}
