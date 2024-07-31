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
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        

        public ProductController(IProductService productService, ILogger<ProductController> logger, IMapper mapper, IUserService userService)
        {
            _productService = productService;
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
         
        }

        // To store some already created products
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

        //View a product by its id
        [HttpGet("GetByID/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound("Product not found!");
                }
                var prod = _mapper.Map<ProductDTO>(product);
                return Ok(prod);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
            
           
        }

        //Adding a product
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            try
            {
                //Mapping the ProductDTO to product entity
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

        //View all products
        [HttpGet("ViewProducts")]
        public async Task<IActionResult> ViewProducts()
        {
            try
            {
                var products = await _productService.GetProductsAsync();
                var prod = _mapper.Map<IEnumerable<ProductDTO>>(products);
                return Ok(prod);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        //Removing a product by its id
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

                    return NotFound("Product not found");
                    throw new NotFoundException("Product not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        //Update a product 
        //Mention ID of the product you want to update
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

                    return NotFound("Product not found");
                    throw new NotFoundException("Product not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error Message: {ex.Message}");
                throw; // Rethrow to be caught by middleware
            }
        }

        //In case admin wants to update exisiting product (only their quantity)
        //Mention the option (increment/decrement) in URL
        [HttpPut("UpdateStock/{id}/{option}/{quantity}")]
        public async Task<IActionResult> UpdateStock(int id, string option, int quantity)
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
                        return NotFound("Product not found");
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

                        return NotFound("Product not found");
                        throw new NotFoundException("Product not found for stock update.");
                    }
                }
                else
                {

                    return BadRequest("Invalid option (select increment/decrement only)");
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
