using POS.Data;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using POS.Repositories.ProductRepository;

namespace POS.Repositories.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly POSDbContext _context;

        public ProductRepository(POSDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id.ToString());
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id.ToString());
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
        public async Task SeedProducts()
        {

            // Check if the database contains any products
            if (!_context.Products.Any())
            {
                // Add initial product data
                await _context.Products.AddRangeAsync(
                    new Product
                    {
                        id = "1",
                        name = "Laptop",
                        price = 899.99,
                        quantity = 10,
                        type = "Electronics",
                        category = "Computers"
                    },
                    new Product
                    {
                        id = "2",
                        name = "Mouse",
                        price = 29.99,
                        quantity = 50,
                        type = "Peripherals",
                        category = "Accessories"
                    },
                    new Product
                    {
                        id = "3",
                        name = "Keyboard",
                        price = 49.99,
                        quantity = 25,
                        type = "Peripherals",
                        category = "Accessories"
                    }
                );

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
        }
        //public async Task View
    }
}
