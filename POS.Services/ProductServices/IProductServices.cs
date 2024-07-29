using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.ProductServices
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<bool> AddProductAsync(Product product);
        Task<bool> UpdateProductAsync(int id,Product product);
        Task<bool> RemoveProductAsync(int id);
        Task<bool> UpdateStockAsync(int productId, int quantity, bool isIncrement);
        Task ViewProductsAsync();
        Task SeedProducts();
    }
}
