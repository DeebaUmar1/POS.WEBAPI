using POS.Models.Entities;
using POS.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }
        public async Task ViewProductsAsync()
        {
            var products = await _repository.GetAllAsync();
            if (products.Count == 0)
            {
                Console.WriteLine("No products available.");
            }
            else
            {
                Console.WriteLine("Products List:");
                Console.WriteLine(new string('-', 80));
                Console.WriteLine($"{"ID",-5} {"Name",-20} {"Price",-10} {"Quantity",-10} {"Type",-15} {"Category",-15}");
                Console.WriteLine(new string('-', 80));

                foreach (var product in products)
                {
                    if (product.quantity != 0)
                    {
                        Console.WriteLine($"{product.id,-5} {product.name,-20} {product.price,-10:C} {product.quantity,-10} {product.type,-15} {product.category,-15}");
                    }
                }

                Console.WriteLine(new string('-', 80));
            }

        }
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> AddProductAsync(Product prod)
        {
            if (string.IsNullOrEmpty(prod.name) || string.Equals(prod.name, "string"))
            {
                return false;
            }
            if (string.IsNullOrEmpty(prod.category) || string.Equals(prod.category, "string"))
            {
                return false;
            }
            if (string.IsNullOrEmpty(prod.type) || string.Equals(prod.type, "string"))
            {
                return false;
            }
            if (prod.quantity <= 0)
            {
                return false;
            }
            if (prod.price <= 0.0)
            {
                return false;
            }
            await _repository.AddAsync(prod);
            return true;
        }

        public async Task<bool> UpdateProductAsync(int id, Product prod)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                    return false;
                // Update product name if it's not null, empty, or "string"
                if (!string.IsNullOrEmpty(prod.name) && !string.Equals(prod.name, "string"))
                {
                    product.name = prod.name;
                }

                // Update product category if it's not null, empty, or "string"
                if (!string.IsNullOrEmpty(prod.category) && !string.Equals(prod.category, "string"))
                {
                    product.category = prod.category;
                }

                // Update product type if it's not null, empty, or "string"
                if (!string.IsNullOrEmpty(prod.type) && !string.Equals(prod.type, "string"))
                {
                    product.type = prod.type;
                }

                // Update product quantity if it's greater than 0
                if (prod.quantity > 0)
                {
                    product.quantity = prod.quantity;
                }

                // Update product price if it's greater than 0
                if (prod.price > 0.0)
                {
                    product.price = prod.price;
                }
                await _repository.UpdateAsync(product);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return false;

        }

        public async Task<bool> RemoveProductAsync(int id)
        {
            var existingProduct = await _repository.GetByIdAsync(id);
            if (existingProduct == null)
                return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity, bool isIncrement)
        {
            var product = await _repository.GetByIdAsync(productId);
            if (product != null)
            {
                if (quantity == product.quantity)
                {
                    await _repository.DeleteAsync(productId);
                    return true;
                }
                
                if (isIncrement)
                    product.quantity += quantity;
                else
                    product.quantity -= quantity;


                await _repository.UpdateAsync(product);
                
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task SeedProducts()
        {
            await _repository.SeedProducts();
        }
    }
}
