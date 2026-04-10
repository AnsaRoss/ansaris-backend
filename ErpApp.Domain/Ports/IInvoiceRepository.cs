using ErpApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Domain.Ports
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice);
        Task<Invoice> GetByIdAsync(int id);

        Task UpdateAsync(Invoice invoice);
        Task SaveChangesAsync();
        Task<IEnumerable<Invoice>> GetAllAsync();
    }
}
