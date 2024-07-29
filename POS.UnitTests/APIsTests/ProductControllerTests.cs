using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using POS.Models.DTO;
using POS.Models.Entities;
using POS.Services;
using POS.Services.ProductServices;
using POS.Services.UserServices;
using POS.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.Tests
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<ProductController>> _loggerMock;
       
        private ProductController _controller;
        private Mock<IProductService> _mockProductService;

        [SetUp]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<ProductController>>();
           _mockProductService = new Mock<IProductService>();

            _controller = new ProductController(_mockProductService.Object, _loggerMock.Object,  _mapperMock.Object, _userServiceMock.Object);
        }

        [Test]
        public async Task SeedProducts_ShouldReturnOk_WhenProductsSeededSuccessfully()
        {
            // Arrange
            _mockProductService.Setup(service => service.SeedProducts()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SeedProducts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Products seeded", ((OkObjectResult)result).Value);
        }
        [Test]
        public async Task AddProduct_ShouldReturnOk_WhenProductAddedSuccessfully()
        {
            // Arrange
            var productDto = new ProductDTO { /* set properties */ };
            var product = new Product { /* set properties */ };

            _mapperMock.Setup(mapper => mapper.Map<Product>(productDto)).Returns(product);
            _mockProductService.Setup(service => service.AddProductAsync(product)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddProduct(productDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Product added", ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task ViewProducts_ShouldReturnOk_WithProductList()
        {
            // Arrange
            var products = new List<Product> { /* list of products */ };
            { /* list of product DTOs */ };

            _mockProductService.Setup(service => service.GetProductsAsync()).ReturnsAsync(products);
            var productDtos = new ProductDTO();
            _mapperMock.Setup(mapper => mapper.Map<ProductDTO>(products)).Returns(productDtos);

            // Act
            var result = await _controller.ViewProducts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(productDtos, ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task RemoveProduct_ShouldReturnOk_WhenProductRemovedSuccessfully()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(service => service.RemoveProductAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveProduct(productId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Product removed", ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task UpdateProduct_ShouldReturnOk_WhenProductUpdatedSuccessfully()
        {
            // Arrange
            int productId = 1;
            var updateProductDto = new UpdateProductDTO { /* set properties */ };
            var product = new Product { /* set properties */ };

            _mapperMock.Setup(mapper => mapper.Map<Product>(updateProductDto)).Returns(product);
            _mockProductService.Setup(service => service.UpdateProductAsync(productId, product)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProduct(productId, updateProductDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Product updated", ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task UpdateStock_ShouldReturnOk_WhenStockUpdatedSuccessfully()
        {
            // Arrange
            int productId = 1;
            int quantity = 10;
            string option = "increment";

            _mockProductService.Setup(service => service.UpdateStockAsync(productId, quantity, true)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateStock(productId, option, quantity);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Product stock updated", ((OkObjectResult)result).Value);
        }

        [Test]
        public void AddProduct_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var productDto = new ProductDTO();
            _controller.ModelState.AddModelError("Error", "Model state is invalid");

            // Act
            var result = _controller.AddProduct(productDto).Result;

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Invalid Data", ((BadRequestObjectResult)result).Value);
        }
    }
}