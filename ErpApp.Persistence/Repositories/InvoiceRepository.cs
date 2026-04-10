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
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .OrderByDescending(i => i.Date)
                .ThenByDescending(i => i.SequenceNumber)
                .ToListAsync();
        }

        public async Task<int> GetNextSequenceNumberAsync(string series, DateTime date)
        {
            var currentMax = await _context.Invoices
                .Where(i => i.Series == series && i.Date.Year == date.Year)
                .Select(i => (int?)i.SequenceNumber)
                .MaxAsync() ?? 0;

            return currentMax + 1;
        }

        public Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            return Task.CompletedTask;
        }
    }
}