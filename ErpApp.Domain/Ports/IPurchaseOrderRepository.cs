using ErpApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Ports
{
    public interface IPurchaseOrderRepository
    {
        Task AddAsync(PurchaseOrder order);
        Task<PurchaseOrder?> GetByIdAsync(string orderNumber);
        Task<List<PurchaseOrder>> GetAllAsync();
        Task SaveChangesAsync();
        Task UpdateAsync(PurchaseOrder order);
        Task<PurchaseOrder?> GetByOrderNumberAsync(string orderNumber);

    }

}
