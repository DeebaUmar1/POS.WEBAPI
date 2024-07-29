using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.UserServices
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();

        Task<bool> RegisterUserAsync(User user);
        Task<User> Login(string name, string password);

        Task SeedUsers();

        Task ViewUsers();

        Task<User> GetUserById(int id);
        Task<bool> UpdateUserRole(string username, UserRole role);
    }
}
