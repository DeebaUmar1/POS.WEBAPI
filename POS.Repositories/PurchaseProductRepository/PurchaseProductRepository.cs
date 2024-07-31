using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using POS.Data;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Repositories.PurchaseProductRepository
{
    public class PurchaseProductRepository : IPurchaseProductRepository
    {
        private readonly POSDbContext _context;

        public PurchaseProductRepository(POSDbContext context)
        {
            _context = context;
        }

     
        public async Task AddAsync(SaleProducts product)
        {

            _context.SaleProducts.Add(product);
            await _context.SaveChangesAsync();
           
        }
        public async Task<List<PurchaseProducts>> GetPurchaseProducts()
        {
            return await _context.PurchaseProducts.ToListAsync();
        }

        public async Task<List<SaleProducts>> GetAllAsync()
        {
            return await _context.SaleProducts.ToListAsync();
           // throw new NotImplementedException();
        }
        public async Task<PurchaseProducts> GetByIdAsync(int id)
        {
            return await _context.PurchaseProducts.FindAsync(id.ToString());
        }
        public async Task UpdateAsync(PurchaseProducts product)
        {
            _context.PurchaseProducts.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAll(List<SaleProducts> salesProducts)
        {
            _context.SaleProducts.RemoveRange(salesProducts);
            _context.SaveChanges();
        }
    }
}
