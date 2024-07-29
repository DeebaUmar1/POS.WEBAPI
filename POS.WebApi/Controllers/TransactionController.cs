using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.Models.DTO;
using POS.Models.Entities;
using POS.Services.TransactionServices;
using static POS.Middlewares.Middlewares.CustomExceptions;

namespace POS.WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("AddProductToSale/{id}/{quantity}")]
        public async Task<IActionResult> AddProductToSale(int id, int quantity)
        {
            try
            {
                bool added = await _transactionService.AddProductToSaleAsync(id, quantity);
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

        [HttpGet("ViewProductsinSale")]
        public async Task<IActionResult> ViewSaleProducts()
        {
            try
            {
                var saleProducts = await _transactionService.GetSaleProductsAsync();
                var saleProductsDTO = _mapper.Map<List<SaleProductsDTO>>(saleProducts);

                if (saleProductsDTO.Count == 0)
                {
                    throw new NotFoundException("No products found in sale");
                }

                _logger.LogInformation($"Products Count in Sale: {saleProductsDTO.Count}");
                return Ok(saleProductsDTO);
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

        [HttpPut("UpdateProductsInSale/{id}/{quantity}")]
        public async Task<IActionResult> UpdateProductsInSale(int id, int quantity)
        {
            try
            {
                bool updated = await _transactionService.UpdateProductinSaleAsync(id, quantity);
                if (updated)
                {
                    _logger.LogInformation("Product updated in sale");
                    return Ok("Product updated in sale");
                }
                else
                {
                    throw new ValidationException("This product is not in sale or invalid quantity");
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

        [HttpGet("GenerateReceipt")]
        public async Task<IActionResult> GenerateReceipt()
        {
            try
            {
                var receipt = await _transactionService.GenerateReceipt();

                if (receipt.Count == 0)
                {
                    throw new NotFoundException("No products found in sale");
                }

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

        [HttpGet("CalculateTotalAmount")]
        public async Task<IActionResult> CalculateTotalAmount()
        {
            try
            {
                double totalAmount = await _transactionService.CalculateTotalAmount();
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
    }
}
