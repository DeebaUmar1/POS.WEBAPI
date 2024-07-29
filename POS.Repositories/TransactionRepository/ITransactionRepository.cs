using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Repositories.TransactionRepository {
    public interface ITransactionRepository
    {

        Task<List<SaleProducts>> GetAllAsync();
        Task AddAsync(SaleProducts product);
        Task UpdateAsync(SaleProducts product);
        Task DeleteAsync(int id);
        Task<SaleProducts> GetByIdAsync(int id);

        void RemoveAll(List<SaleProducts> salesProducts);
    }
}
