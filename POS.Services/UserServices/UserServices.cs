using Microsoft.EntityFrameworkCore;
using POS.Models.Entities;
using POS.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.UserServices
{
    public class UserService : IUserService
    {

        private readonly IUserRepository userRepository;
        public UserService(IUserRepository repository)
        {
            userRepository = repository;
        }
        public async Task<List<User>> GetUsersAsync()
        {

            return await userRepository.GetAllAsync();
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {
                // Fetch users asynchronously
                var users = await GetUsersAsync();

                // Check if the list of users is null to avoid potential NullReferenceException
                if (users == null)
                {
                    await userRepository.AddAsync(user);
                    return true;
                    //throw new InvalidOperationException("Failed to fetch users.");
                }

                // Check if the user already exists
                if (users.Any(u => u.name == user.name || u.email == user.email))
                {
                    return false; // User already exists
                }

                // Add the new user
                await userRepository.AddAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow
             
                throw; // Let the calling code handle the exception
            }
        }

        public async Task<User> Login(string name, string password)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            {
                
                return null;
            }
            else
            {
                User user  = await userRepository.LogInAsync(name, password);
                if (user != null)
                {
                    return user;
                }

                else
                {
                    return null;
                }
            }
        }

        public async Task<bool> UpdateUserRole(string username, UserRole role)
        {
            return await userRepository.UpdateUserRoleAsync(username, role);
        }
        public async Task<User> GetUserById(int id)
        {
            return await userRepository.GetUserByIdAsync(id);
        }
        public async Task SeedUsers()
        {
            await userRepository.SeedUsersAsync();
        }

        public async Task ViewUsers()
        {
            Console.Clear();
            Console.WriteLine("User List:");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("ID\tName\t\tEmail\t\tRole");
            Console.WriteLine("------------------------------------------------");
            var users = GetUsersAsync();
            foreach (var user in await users)
            {
                Console.WriteLine($"{user.Id}\t{user.name}\t\t{user.email}\t\t{user.role}");
            }

            Console.WriteLine("------------------------------------------------");
        }
    }
}
