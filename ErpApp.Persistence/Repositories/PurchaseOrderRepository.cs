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
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext _context;

        public PurchaseOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PurchaseOrder order)
        {
            await _context.PurchaseOrders.AddAsync(order);
        }

        public async Task<PurchaseOrder?> GetByIdAsync(string orderNumber)
        {
            return await _context.PurchaseOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<List<PurchaseOrder>> GetAllAsync()
        {
            return await _context.PurchaseOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<PurchaseOrder?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.PurchaseOrders
                .Include(o => o.Items)               // Incluye detalles si quieres
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }
        public async Task UpdateAsync(PurchaseOrder order)
        {
            _context.PurchaseOrders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
