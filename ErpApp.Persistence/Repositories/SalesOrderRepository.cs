using ErpApp.Domain.Entities;
using ErpApp.Domain.Ports;
using ERPApp.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpApp.Persistence.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly AppDbContext _context;

        public SalesOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SalesOrder order)
        {
            await _context.SalesOrders.AddAsync(order);
        }

        public async Task<SalesOrder?> GetByIdAsync(string orderNumber)
        {
            return await _context.SalesOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }
        public async Task<List<SalesOrder>> GetAllAsync()
        {
            return await _context.SalesOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(SalesOrder order)
        {
            _context.SalesOrders.Update(order);
            await _context.SaveChangesAsync();
        }
        public async Task<SalesOrder?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.SalesOrders
                .Include(o => o.Items)               
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        
    }

}
