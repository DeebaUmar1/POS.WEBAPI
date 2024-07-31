using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Repositories.ProductRepository
{
    public class ProductCosmosRepository : IProductRepository
    {
        private readonly Container _container;
        private readonly Container _counterContainer;
        public ProductCosmosRepository(CosmosClient cosmosClient)
        {
            _container = cosmosClient.GetContainer("POS", "Products");
            _counterContainer = cosmosClient.GetContainer("POS", "Counter");
        }


        public async Task AddAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            // Cosmos DB requires string id
            product.id = await GetNextIdAsync(); 

            await _container.CreateItemAsync(product, new PartitionKey(product.id));
        }

        public async Task DeleteAsync(int id)
        {
            
            var partitionKey = id.ToString();
            await _container.DeleteItemAsync<Product>(id.ToString(), new PartitionKey(partitionKey));
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var queryIterator = _container.GetItemQueryIterator<Product>(query);
            var products = new List<Product>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                products.AddRange(response.ToList());
            }

            return products;
        }

        //The counter container has item with id and Value, id stores the counter identifier
        //while Value is the id of the current product in Products Container
        private async Task<string> GetNextIdAsync()
        {
            var counterId = "product-counter";
            // Identifier for the counter

            try
            {
                // Read the existing counter item
                var response = await _counterContainer.ReadItemAsync<Counter>(counterId, new PartitionKey(counterId));

                // Update the counter value
                var counter = response.Resource;
                counter.Value++;
                // Replace the existing counter item
                await _counterContainer.ReplaceItemAsync(counter, counterId, new PartitionKey(counterId));

                return counter.Value.ToString(); // Convert to string
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Initialize the counter if it does not exist
                var counter = new Counter { id = counterId, Value = 1 };
                // Create a new counter item
                await _counterContainer.CreateItemAsync(counter, new PartitionKey(counter.id));

                return counter.Value.ToString(); // Convert to string
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                throw;
            }
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Product>(id.ToString(), new PartitionKey(id.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task SeedProducts()
        {
           
            var products = new List<Product>
            {
                new Product
                    {
                       
                        name = "Laptop",
                        price = 899.99,
                        quantity = 10,
                        type = "Electronics",
                        category = "Computers"
                    },
                    new Product
                    {
                        
                        name = "Mouse",
                        price = 29.99,
                        quantity = 50,
                        type = "Peripherals",
                        category = "Accessories"
                    },
                    new Product
                    {
                        
                        name = "Keyboard",
                        price = 49.99,
                        quantity = 25,
                        type = "Peripherals",
                        category = "Accessories"
                    }
            };

            foreach (var product in products)
            {
                await AddAsync(product);
            }
        }

        public async Task UpdateAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await _container.UpsertItemAsync(product, new PartitionKey(product.id.ToString()));
        }
    }
}
