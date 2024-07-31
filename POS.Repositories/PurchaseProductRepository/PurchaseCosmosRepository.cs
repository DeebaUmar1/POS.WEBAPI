using Microsoft.Azure.Cosmos;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Repositories.PurchaseProductRepository
{
    public class PurchaseProductCosmosRepository : IPurchaseProductRepository
    {
        private readonly Container container;
        private readonly Container saleProductsContainer;
        private readonly Container _counterContainer;
        public PurchaseProductCosmosRepository(CosmosClient client)
        {
            container = client.GetContainer("POS", "PurchaseProducts");
            saleProductsContainer = client.GetContainer("POS", "SaleProducts");
            _counterContainer = client.GetContainer("POS", "Counter");
        }

        //Adds the product admin wants to purchase
        public async Task AddAsync(SaleProducts product)
        {
            try
            {
               product.id = await GetNextIdAsync();
                
                await saleProductsContainer.CreateItemAsync(product, new PartitionKey (product.id.ToString()));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.Message);
               
            }
        }

        //get all the products currently in the sale
        public async Task<List<SaleProducts>> GetAllAsync()
        {
            try
            {
                var query = new QueryDefinition("SELECT * FROM c");
                var iterator = saleProductsContainer.GetItemQueryIterator<SaleProducts>(query);
                var results = new List<SaleProducts>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }

                return results;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //Gets a purchase product (provided by a supplier) by ID
        public async Task<PurchaseProducts> GetByIdAsync(int id)
        {
            try
            {
                var response = await container.ReadItemAsync<PurchaseProducts>(id.ToString(), new PartitionKey(id.ToString()));
                return response.Resource;
            

               
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //All purchase products provided byt the supplier
        public async Task<List<PurchaseProducts>> GetPurchaseProducts()
        {
            try
            {
                var query = new QueryDefinition("SELECT * FROM c");
                var iterator = container.GetItemQueryIterator<PurchaseProducts>(query);
                var results = new List<PurchaseProducts>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    results.AddRange(response);
                }

                return results;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //Update the purchase products 
        //After admin adds that product to sale
        public async Task UpdateAsync(PurchaseProducts product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await container.UpsertItemAsync(product, new PartitionKey(product.id.ToString()));
        }


        //To remove the products from sale after generating receipt
        public async Task RemoveAll(List<SaleProducts> salesProducts)
        {
            foreach (var product in salesProducts)
            {
                try
                {
                    await saleProductsContainer.DeleteItemAsync<SaleProducts>(product.id.ToString(), new PartitionKey(product.id.ToString()));
                }
                catch (CosmosException ex)
                {
                    Console.WriteLine($"Cosmos DB error: {ex.Message}");
                    // Handle exception as needed
                }
            }
        }
        //To get the next id to save sale product
        private async Task<string> GetNextIdAsync()
        {
            var counterId = "purchase-counter";
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

    }
}
