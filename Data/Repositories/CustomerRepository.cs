using Data.Data;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly TradeMarketDbContext _context;

        public CustomerRepository(TradeMarketDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer entity)
        {
            await _context.Customers.AddAsync(entity);
        }

        public void Delete(Customer entity)
        {
            _context.Customers.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var existingEntity = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (existingEntity != null)
            {
                _context.Customers.Remove(existingEntity);
            }
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllWithDetailsAsync()
        {
            return await _context.Customers.Include(c => c.Person)
                .Include(c => c.Receipts)
                .ThenInclude(r => r.ReceiptDetails)
                .ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Customers.Include(c => c.Person)
                .Include(c => c.Receipts)
                .ThenInclude(c => c.ReceiptDetails)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Update(Customer entity)
        {
            entity.Person = null;
            _context.Customers.Update(entity);
        }
    }
}
