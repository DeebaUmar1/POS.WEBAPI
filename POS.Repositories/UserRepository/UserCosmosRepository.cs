using Microsoft.Azure.Cosmos;
using POS.Models.Entities;
using POS.Validation;
using User = POS.Models.Entities.User;

namespace POS.Repositories.UserRepository
{
    public class UserCosmosRepository : IUserRepository
    {
        private readonly Container _userContainer;
        private readonly Container _counterContainer;

        public UserCosmosRepository(CosmosClient client)
        {
            _userContainer = client.GetContainer("POS", "Users");
            _counterContainer = client.GetContainer("POS", "Counter");
        }

        public async Task<bool> AddAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            // Get a new unique ID
            user.id = await GetNextIdAsync();

            // Convert ID to string
            var partitionKey = user.id;
            try
            {
                await _userContainer.CreateItemAsync(user, new PartitionKey(partitionKey));
                return true;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }
            return false;
        }

        private async Task<string> GetNextIdAsync()
        {
            var counterId = "user-counter";
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



        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();

            try
            {
                var query = new QueryDefinition("SELECT * FROM c");
                var queryIterator = _userContainer.GetItemQueryIterator<User>(query);

                while (queryIterator.HasMoreResults)
                {
                    var response = await queryIterator.ReadNextAsync();
                    users.AddRange(response.ToList());
                }
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }

            return users;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var partitionKey = id.ToString();

            try
            {
                var response = await _userContainer.ReadItemAsync<User>(partitionKey, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<User> LogInAsync(string name, string password)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.name = @name")
                .WithParameter("@name", name);

            var queryIterator = _userContainer.GetItemQueryIterator<User>(query);

            try
            {
                while (queryIterator.HasMoreResults)
                {
                    var response = await queryIterator.ReadNextAsync();
                    var user = response.FirstOrDefault();

                    if (user != null)
                    {
                        string encryptedPassword = user.password;
                        string decryptedPassword = Password.DecodeFrom64(encryptedPassword);

                        if (password == decryptedPassword)
                        {
                            return user;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Cosmos DB error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
            }

            return null;
        }

        public async Task SeedUsersAsync()
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.name IN ('admin', 'cashier', 'manager')");

            var queryResultSetIterator = _userContainer.GetItemQueryIterator<User>(query);
            var existingUsers = await queryResultSetIterator.ReadNextAsync();


            if (!existingUsers.Any())
            {
                var users = new List<User>
                {
                    new User { name = "admin", email = "email", password = Password.EncodePasswordToBase64("adminpass"), role = UserRole.Admin },
                    new User { name = "cashier", email = "email", password = Password.EncodePasswordToBase64("cashierpass"), role = UserRole.Cashier },
                    new User { name = "manager", email = "email", password = Password.EncodePasswordToBase64("managerpass"), role = UserRole.Admin }
                };
                foreach (var user in users)
                {
                    user.id = await GetNextIdAsync(); // Assign ID from the counter
                    await _userContainer.CreateItemAsync(user, new PartitionKey(user.id));
                }
            }
        }

        public async Task<bool> UpdateUserRoleAsync(string username, UserRole role)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.name = @name")
                            .WithParameter("@name", username);

            var queryResultSetIterator = _userContainer.GetItemQueryIterator<User>(query);
            var user = (await queryResultSetIterator.ReadNextAsync()).FirstOrDefault();

            if (user == null)
                return false;

            user.role = role;

            var partitionKey = user.id.ToString();
            var response = await _userContainer.UpsertItemAsync(user, new PartitionKey(partitionKey));
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }

}
