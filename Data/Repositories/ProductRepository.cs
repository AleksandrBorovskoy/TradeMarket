﻿using Data.Data;
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
    public class ProductRepository : IProductRepository
    {
        private readonly TradeMarketDbContext _context;

        public ProductRepository(TradeMarketDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product entity)
        {
            entity.Category = null;
            await _context.Products.AddAsync(entity);
        }

        public void Delete(Product entity)
        {
            _context.Products.Remove(entity);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var existingEntity = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);

            if (existingEntity != null)
            {
                _context.Products.Remove(existingEntity);
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _context.Products.Include(p => p.Category)
                .Include(p => p.ReceiptDetails)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Products.Include(p => p.Category)
                .Include(p => p.ReceiptDetails)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public void Update(Product entity)
        {
            entity.Category = null;
            _context.Products.Update(entity);
        }
    }
}
