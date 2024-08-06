using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Data
{
    public class CosmosDbSetup
    {
        private readonly CosmosClient _cosmosClient;
        private const string DatabaseName = "POS";
        private const string ContainerName1 = "Users";
        private const string ContainerName2 = "Products";
        private const string ContainerName3 = "SaleProducts";
        private const string ContainerName4 = "PurchaseProducts";
        private const string ContainerName5 = "Counter";

        public CosmosDbSetup(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task CreateDatabaseAndContainerAsync()
        {
            // Create Database
            var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);
            Database database = databaseResponse.Database;

            var containerResponse1 = await database.CreateContainerIfNotExistsAsync(
                id: ContainerName1,
                partitionKeyPath: "/id"  
            );
            Container container1 = containerResponse1.Container;
           
            var containerResponse2 = await database.CreateContainerIfNotExistsAsync(
               id: ContainerName2,
               partitionKeyPath: "/id"  
           );
            Container container2 = containerResponse2.Container;

            var containerResponse3 = await database.CreateContainerIfNotExistsAsync(
             id: ContainerName3,
             partitionKeyPath: "/id"
         );
            Container container3 = containerResponse3.Container;

            var containerResponse4 = await database.CreateContainerIfNotExistsAsync(
          id: ContainerName4,
          partitionKeyPath: "/id"   
      );
            Container container4 = containerResponse4.Container;

            var containerResponse5 = await database.CreateContainerIfNotExistsAsync(
             id: ContainerName5,
        partitionKeyPath: "/id"
  );
            Container container5 = containerResponse5.Container;


        }
    }
}
