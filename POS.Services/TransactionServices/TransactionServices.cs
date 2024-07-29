using POS.Models.Entities;
using POS.Repositories.TransactionRepository;
using POS.Services.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Services.TransactionServices
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly ProductService productService;
        public TransactionService(ITransactionRepository transactionRepository, ProductService productService)
        {
            this.transactionRepository = transactionRepository;
            this.productService = productService;
        }
        public async Task<bool> AddProductToSaleAsync(int productId, int quantity)
        {
            var product = await productService.GetProductByIdAsync(productId);

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

                product.quantity -= quantity;
                await transactionRepository.AddAsync(sale);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<SaleProducts>> GetSaleProductsAsync()
        {
            return await transactionRepository.GetAllAsync();
        }

        public async Task<SaleProducts> GetProductByIdAsync(int id)
        {
            return await transactionRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateProductinSaleAsync(int productId, int quantity)
        {

            var sale = GetProductByIdAsync(productId);
            var saleProductToUpdate = sale.Result;
            if (saleProductToUpdate != null)
            {
                var prod = productService.GetProductByIdAsync(productId);
                var originalProduct = prod.Result;
                if (originalProduct != null)
                {
                    int originalSaleQuantity = saleProductToUpdate.Quantity;
                    int availableQuantity = originalProduct.quantity + originalSaleQuantity;

                    if (quantity < 0 || quantity > availableQuantity)
                    {
                        return false;
                    }
                    originalProduct.quantity += originalSaleQuantity; // Restore the original quantity
                    saleProductToUpdate.Quantity = quantity;

                    if (quantity == 0)
                    {
                        await RemoveProductFromSaleAsync(saleProductToUpdate.SalesTransactionId);
                    }
                    else
                    {
                        originalProduct.quantity -= quantity;
                        await productService.UpdateProductAsync(originalProduct.Id, originalProduct);
                    }

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                Console.WriteLine("Product does not exists!");
                return false;

            }
        }
        public async Task<bool> RemoveProductFromSaleAsync(int id)
        {
            await transactionRepository.DeleteAsync(id);
            return true;
        }

        /*public void Generate()
        {
            Console.Write("Do you want to generate receipt? (y/n): ");
            string? response2 = Console.ReadLine();
            while (response2 == null || response2.Trim().ToLower() != "y" && response2.Trim().ToLower() != "n")
            {
                Console.WriteLine("Invalid response! Enter y or n: ");
                response2 = Console.ReadLine();
            }

            if (response2.Trim().ToLower() == "y")
            {
                GenerateReceipt();
            }
            else
            {
*//*
                Cashier cashier = new Cashier(this, productService);
                cashier.ShowCashierMenu();*//*
            }
        }

        public void PrintTotalAmount()
        {
            Console.WriteLine($"{"Total Amount:",-30} {CalculateTotalAmount():C}");
        }*/

        public async Task<double> CalculateTotalAmount()
        {
            var products = await GetSaleProductsAsync();

            var total = products.Sum(s => s.Quantity * s.ProductPrice);
            if (total > 0)
            {
                return total;
            }
            else
            {
                Console.WriteLine("Please add products to sale before calculating total amount!");
                return 0.0;
            }

        }
        public async Task<List<FinalReceipt>> GenerateReceipt()
        {
            var saleProducts = await transactionRepository.GetAllAsync();
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

                transactionRepository.RemoveAll(saleProducts);

            }

            return totalReceipt;
        }
    /*    public async Task<List<FinalReceipt>> GenerateReceipt()
        {
            var saleProducts = await transactionRepository.GetAllAsync();
            var receipt = new List<Receipt>();
            var totalReceipt = new List<FinalReceipt>();

            if (saleProducts.Any())
            {
                Console.WriteLine("Receipt:");
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"{"Quantity",-10} {"Product",-20} {"Price",-10} {"Total",-10}");
                Console.WriteLine(new string('-', 50));

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
                    Console.WriteLine($"{sale.Quantity,-10} {sale.ProductName,-20} {sale.ProductPrice,-10:C} {totalPrice,-10}");
                }
                var total = CalculateTotalAmount();
                double t = total.Result;
                string totalAmount = Convert.ToString(t);
                Console.WriteLine(new string('-', 50));
                Console.WriteLine($"{"Total Amount:",-30} {totalAmount}");
                totalReceipt.Add(new FinalReceipt
                {
                    Receipt = receipt,
                    TotalAmount = totalAmount
                });

                Console.WriteLine(new string('-', 50));

                // Remove all sale products
                transactionRepository.RemoveAll(saleProducts);

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Please add products to sale before generating receipt.");
            }

            return totalReceipt;
        }*/
    }
}
