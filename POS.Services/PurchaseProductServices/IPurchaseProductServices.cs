using POS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.PurchaseProductServices
{
    public interface IPurchaseProductServices
    {
        Task<double> CalculateTotalAmount();
        Task<List<FinalReceipt>> GenerateReceipt();
       Task<List<SaleProducts>> GetAll();
        Task<bool> AddPurchaseProducts(int productId, int quantity);

        Task<List<PurchaseProducts>> ViewPurchaseProductsAsync();
        Task<bool> UpdateStockAsync(int productId, int quantity, bool isIncrement);

    }
}
