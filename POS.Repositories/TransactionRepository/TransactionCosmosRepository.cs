using Microsoft.Azure.Cosmos;
using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POS.Repositories.TransactionRepository
{
    public class TransactionCosmosRepository : ITransactionRepository
    {
        private readonly Container _container;
        private readonly Container _counterContainer;

        public TransactionCosmosRepository(CosmosClient cosmosClient)
        {
            _container = cosmosClient.GetContainer("POS", "SaleProducts");
            _counterContainer = cosmosClient.GetContainer("POS", "Counter");
        }

        public async Task<SaleProducts> GetByIdAsync(int id)
        {
            try
            {
                var response = await _container.ReadItemAsync<SaleProducts>(id.ToString(), new PartitionKey(id.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Handle the case where the item was not found
                return null;
            }
        }

        public async Task<List<SaleProducts>> GetAllAsync()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = _container.GetItemQueryIterator<SaleProducts>(query);
            var results = new List<SaleProducts>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task AddAsync(SaleProducts product)
        {
            try
            {
                product.id = await GetNextIdAsync();
                await _container.CreateItemAsync(product, new PartitionKey(product.id.ToString()));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
                
            }
        }

        private async Task<string> GetNextIdAsync()
        {
            var counterId = "sale-counter";
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

        public async Task UpdateAsync(SaleProducts product)
        {
            try
            {
                await _container.UpsertItemAsync(product, new PartitionKey(product.id.ToString()));
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _container.DeleteItemAsync<SaleProducts>(id.ToString(), new PartitionKey(id.ToString()));
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception("Sale Product not found");
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        public  void  RemoveAll(List<SaleProducts> salesProducts)
        {
            foreach (var product in salesProducts)
            {
                try
                {
                     _container.DeleteItemAsync<SaleProducts>(product.id.ToString(), new PartitionKey(product.id.ToString()));
                }
                catch (CosmosException ex)
                {
                    Console.WriteLine($"Cosmos DB error: {ex.Message}");
                    throw new Exception(ex.Message);
                }
            }
        }

      }
    }

