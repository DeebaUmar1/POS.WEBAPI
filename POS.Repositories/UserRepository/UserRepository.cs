using Microsoft.EntityFrameworkCore;
using POS.Data;
using POS.Models.Entities;
using POS.Validation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly POSDbContext context;
        public UserRepository(POSDbContext context)
        {
            this.context = context;
        }
        public async Task<List<User>> GetAllAsync()
        {
           var users = await context.Users.ToListAsync();
            if(users.Count == 0)
            {
                return null;
            }
            else
            {
                return users;
            }
            //return await context.Users.ToListAsync() ?? new List<User>(); 
        }
        public async Task AddAsync(User user)
        {
            var users = GetAllAsync().Result;
            if (users == null)
            {
                user.Id = 1;
            }
            else
            { 
                user.Id = context.Users.Max(u => u.Id) + 1;
            }
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
        public async Task<User> LogInAsync(string name, string password)
        {
            var searchResults = await context.Users.FirstOrDefaultAsync(user => user.name == name);
            if (searchResults == null)
            {
                return null; // User not found
            }

            string encryptedPassword = searchResults.password;
            string decryptedPassword = Password.DecodeFrom64(encryptedPassword);

            if (password == decryptedPassword)
            {
                return searchResults; // Return the user's role
            }
            else
            {
                return null; // Incorrect password
            }
        }
        public async Task<bool> UpdateUserRoleAsync(string username, UserRole role)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.name == username);
            if (user == null)
                return false;

            user.role = role;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return true;
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            
            var user =     await context.Users.FindAsync(id);
            if(user == null)
            {
                return null;
            }
            else
            {

                return user; 
            }
        }
        
        public async Task SeedUsersAsync()
        {
           
            if (!context.Users.Any())
            {
                await context.Users.AddRangeAsync(
                    new User { Id = 1, name = "admin", email = "email", password = Password.EncodePasswordToBase64("adminpass"), role = UserRole.Admin },
                    new User { Id = 2, name = "cashier", email = "email", password = Password.EncodePasswordToBase64("cashierpass"), role = UserRole.Cashier },
                    new User { Id = 3, name = "manager", email = "email", password = Password.EncodePasswordToBase64("managerpass"), role = UserRole.Admin }
                );
                await context.SaveChangesAsync();
            }

        }
    }
}
