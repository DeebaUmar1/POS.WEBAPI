using Microsoft.EntityFrameworkCore;
using POS.Models.Entities;
using POS.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Data
{
    public class POSDbContext :DbContext
    {
        public POSDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<SaleProducts> SaleProducts { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<PurchaseProducts> PurchaseProducts { get; set; }

        public static void SeedData(POSDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { id = "1", name = "admin", email = "email", password = Password.EncodePasswordToBase64("adminpass"), role = UserRole.Admin }

                );
                context.SaveChanges();
            }
        }

      
      
    }
}
