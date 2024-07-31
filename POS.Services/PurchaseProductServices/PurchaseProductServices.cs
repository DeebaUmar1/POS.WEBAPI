using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using POS.Models.Entities;
using POS.Repositories.PurchaseProductRepository;
using POS.Repositories.TransactionRepository;
using POS.Services.PurchaseProductServices;

namespace POS.Services.ProductServices
{
    public  class PurchaseProductServices : IPurchaseProductServices
    {
        private readonly IPurchaseProductRepository purchaseProductRepository;

        private readonly ProductService productService;
        public PurchaseProductServices(IPurchaseProductRepository purchaseProductRepository, ProductService productService)
        {
            this.purchaseProductRepository = purchaseProductRepository;
            this.productService = productService;
        }
        public async Task<List<PurchaseProducts>> ViewPurchaseProductsAsync()
        {
            var products = await purchaseProductRepository.GetPurchaseProducts();
            return products;
          

        }

        public async Task<List<SaleProducts>> GetAll()
        {
            return await purchaseProductRepository.GetAllAsync();
        }
        public async Task<double> CalculateTotalAmount()
        {
            var products = await GetAll();

            var total = products.Sum(s => s.Quantity * s.ProductPrice);
            if (total > 0)
            {
                return total;
            }
            else
            {
                Console.WriteLine("Please add products before calculating total amount!");
                return 0.0;
            }

        }

        public async Task<bool> AddPurchaseProducts(int productId, int quantity)
        {
            var product = await purchaseProductRepository.GetByIdAsync(productId);

            if (product != null)
            {
                if (quantity <= 0 || quantity > product.quantity)
                {
                    return false;
                }

                var sale = new SaleProducts
                {
                  
                    Date = DateTime.Now,
                    Quantity = quantity,
                    ProductId = productId,
                    ProductName = product.name,
                    ProductPrice = product.price
                };

               
                await UpdateStockAsync(Convert.ToInt32(product.id), quantity, false);
                await purchaseProductRepository.AddAsync(sale);
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<FinalReceipt>> GenerateReceipt()
        {
            var saleProducts = await purchaseProductRepository.GetAllAsync();
            var receipt = new List<Receipt>();
            var totalReceipt = new List<FinalReceipt>();
            if (saleProducts.Any())
            {
                foreach (var sale in saleProducts)
                {
                    string totalPrice = (sale.Quantity * sale.ProductPrice).ToString("C");
                    receipt.Add(new Receipt
                    {
                        
                        Quantity = sale.Quantity.ToString(),
                        Product = sale.ProductName,
                        Price = sale.ProductPrice.ToString("C"),
                        Total = totalPrice
                    });
                }
                var total = CalculateTotalAmount();
                double t = total.Result;
                string totalAmount = Convert.ToString(t);
                //var totalAmount = CalculateTotalAmount().ToString();

                totalReceipt.Add(new FinalReceipt
                {
                    Receipt = receipt.ToList(),
                    TotalAmount = totalAmount
                });

                await purchaseProductRepository.RemoveAll(saleProducts);
                await AddProductsToInventory();

            }

            return totalReceipt;
        }
        public async Task AddProductsToInventory()
        {
            var saleProducts = GetAll();
            var products = saleProducts.Result.ToList();
            foreach (var sale in products)
            {
                var originalProduct = purchaseProductRepository.GetByIdAsync(sale.ProductId);
                var product = originalProduct.Result;

                if (product != null)
                {
                    var newProduct = new Product
                    {

                        name = product.name,
                        price = product.price,
                        quantity = sale.Quantity,
                        type = product.type,
                        category = product.category

                    };
                    await productService.AddProductAsync(newProduct);
                }
            }
        }
        public async Task<bool> UpdateStockAsync(int productId, int quantity, bool isIncrement)
        {
            var product = await purchaseProductRepository.GetByIdAsync(productId);
            if (product != null)
            {
                if (isIncrement)
                    product.quantity += quantity;
                else
                    product.quantity -= quantity;

                // Ensure stock is not negative
                if (product.quantity < 0)
                    product.quantity = 0;

                await purchaseProductRepository.UpdateAsync(product);
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
