using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS.Models.DTO;
using POS.Models.Entities;
using POS.Services;
using POS.Services.ProductServices;
using POS.Services.UserServices;
using static POS.Middlewares.Middlewares.CustomExceptions;
using System.Threading.Tasks;
using AutoMapper;

namespace POS.WebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;
        private readonly IMapper _mapper;
        private readonly UserService _userService;
        

        public ProductController(ProductService productService, ILogger<ProductController> logger, IMapper mapper, UserService userService)
        {
            _productService = productService;
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
         
        }

        [HttpPost("SeedProducts")]
        public async Task<IActionResult> SeedProducts()
        {
            try
            {
                await _productService.SeedProducts();
                _logger.LogInformation("Products added!!");
                return Ok("Products seeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                var prod = _mapper.Map<Product>(product);
                if (!ModelState.IsValid)
                {
                    throw new ValidationException("Invalid product data.");
                }

                bool added = await _productService.AddProductAsync(prod);
                if (added)
                {
                    _logger.LogInformation($"Product added!! {product}");
                    return Ok("Product added");
                }
                else
                {
                    _logger.LogWarning("All fields are required");
                    throw new Exception("Failed to add product.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        [HttpGet("ViewProducts")]
        public async Task<IActionResult> ViewProducts()
        {
            try
            {
                var products = await _productService.GetProductsAsync();
                var prod = _mapper.Map<ProductDTO>(products);
                return Ok(prod);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        [HttpDelete("RemoveProduct/{id}")]
        public async Task<IActionResult> RemoveProduct(int id)
        {
            try
            {
                var deleted = await _productService.RemoveProductAsync(id);
                if (deleted)
                {
                    _logger.LogInformation($"Product with id#{id} is removed");
                    return Ok("Product removed");
                }
                else
                {
                    throw new NotFoundException("Product not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                var prod = _mapper.Map<Product>(product);
                bool updated = await _productService.UpdateProductAsync(id, prod);

                if (updated)
                {
                    _logger.LogInformation("Product has been updated!");
                    return Ok("Product updated");
                }
                else
                {
                    throw new NotFoundException("Product not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        [HttpPut("UpdateStock/{id}/{quantity}")]
        public async Task<IActionResult> UpdateStock(int id, [FromQuery] string option, int quantity)
        {
            try
            {
                if (option == "increment")
                {
                    bool increased = await _productService.UpdateStockAsync(id, quantity, true);
                    if (increased)
                    {
                        return Ok("Product stock updated");
                    }
                    else
                    {
                        throw new Exception("Stock update failed.");
                    }
                }
                else if (option == "decrement")
                {
                    bool decreased = await _productService.UpdateStockAsync(id, quantity, false);
                    if (decreased)
                    {
                        return Ok("Stock Updated");
                    }
                    else
                    {
                        throw new NotFoundException("Product not found for stock update.");
                    }
                }
                else
                {
                    throw new ValidationException("Invalid option");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }
    }
}
