using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POS.Models.DTO;
using POS.Models.Entities;
using POS.Repositories.PurchaseProductRepository;
using POS.Services.PurchaseProductServices;
using POS.Services.TransactionServices;
using static POS.Middlewares.Middlewares.CustomExceptions;

namespace POS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseProductServices purchaseProduct;
        private readonly ILogger<PurchaseController> _logger;
        private readonly IMapper _mapper;

        public PurchaseController(IPurchaseProductServices purchaseProduct, ILogger<PurchaseController> logger, IMapper mapper)
        {
            this.purchaseProduct = purchaseProduct;
            _mapper = mapper;
            _logger = logger;
        }

        //Add one of the products from Purchase Products

        [HttpPost("AddProductToPurchase/{id}/{quantity}")]
        public async Task<IActionResult> AddPurchaseProduct(int id, int quantity)
        {
            try
            {
                bool added = await purchaseProduct.AddPurchaseProducts(id, quantity);
                if (added)
                {
                    _logger.LogInformation("Product added to sale");
                    return Ok("Product added to sale");
                }
                else
                {
                    throw new ValidationException("This product does not exist or invalid quantity");
                }
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"Validation error: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        //Generate the reciept of your products.
        //Generating the receipt will remove all the sale products.
        [HttpGet("GenerateReceipt")]
        public async Task<IActionResult> GenerateReceipt()
        {
            try
            {
                var receipt = await purchaseProduct.GenerateReceipt();

                if (receipt.Count == 0)
                {
                    throw new NotFoundException("No products found in sale");
                }
                //This receipt contain all products and their total amount
                var finalReceipt = _mapper.Map<List<FinalReceiptDTO>>(receipt);

                _logger.LogInformation($"Receipt generated with {finalReceipt.Count} items.");
                return Ok(finalReceipt);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError($"Not found error: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        //In case you want to see total amount before generating receipt.
        //This will tell the total amount only.
        [HttpGet("CalculateTotalAmount")]
        public async Task<IActionResult> CalculateTotalAmount()
        {
            try
            {
                double totalAmount = await purchaseProduct.CalculateTotalAmount();
                if (totalAmount == 0)
                {
                    throw new NotFoundException("No products found in sale");
                }
                else
                {
                    _logger.LogInformation($"Total amount: {totalAmount}");
                    return Ok(totalAmount);
                }
            }
            catch (NotFoundException ex)
            {
                _logger.LogError($"Not found error: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        //Before purchasing products, you can view the available products given by the supplier.
        [HttpGet("ViewPurchaseProducts")]
        public async Task<IActionResult> ViewProducts()
        {
            try
            {
                var products = await purchaseProduct.ViewPurchaseProductsAsync();
                var prod = _mapper.Map<IEnumerable<PurchaseProductsDTO>>(products);
                return Ok(prod);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }
    }
}
