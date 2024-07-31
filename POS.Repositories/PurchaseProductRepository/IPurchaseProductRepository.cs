using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Repositories.PurchaseProductRepository
{
    public interface IPurchaseProductRepository
    {
        Task<List<PurchaseProducts>> GetPurchaseProducts();
        Task<List<SaleProducts>> GetAllAsync();
        Task AddAsync(SaleProducts product);

        Task<PurchaseProducts> GetByIdAsync(int id);

        Task UpdateAsync(PurchaseProducts product);

        Task RemoveAll(List<SaleProducts> saleProducts);
    }
}
