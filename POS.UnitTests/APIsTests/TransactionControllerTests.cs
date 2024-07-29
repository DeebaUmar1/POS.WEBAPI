using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using POS.Models.DTO;
using POS.Models.Entities;
using POS.Services.TransactionServices;
using POS.WebApi.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static POS.Middlewares.Middlewares.CustomExceptions;

namespace POS.Tests.Controllers
{
    [TestFixture]
    public class TransactionControllerTests
    {
        private Mock<ITransactionService> _mockTransactionService;
        private Mock<ILogger<TransactionController>> _mockLogger;
        private Mock<IMapper> _mockMapper;
        private TransactionController _controller;

        [SetUp]
        public void Setup()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _mockLogger = new Mock<ILogger<TransactionController>>();
            _mockMapper = new Mock<IMapper>();

            _controller = new TransactionController(_mockTransactionService.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Test]
        public async Task AddProductToSale_ShouldReturnOk_WhenProductAddedSuccessfully()
        {
            // Arrange
            int productId = 1;
            int quantity = 5;
            _mockTransactionService.Setup(service => service.AddProductToSaleAsync(productId, quantity)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddProductToSale(productId, quantity);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Product added to sale", ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task AddProductToSale_ShouldReturnBadRequest_WhenValidationExceptionThrown()
        {
            // Arrange
            int productId = 1;
            int quantity = 5;
            _mockTransactionService.Setup(service => service.AddProductToSaleAsync(productId, quantity)).ThrowsAsync(new ValidationException("This product does not exist or invalid quantity"));

            // Act
            var result = await _controller.AddProductToSale(productId, quantity);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("This product does not exist or invalid quantity", ((BadRequestObjectResult)result).Value);
        }

       

        [Test]
        public async Task UpdateProductsInSale_ShouldReturnOk_WhenProductUpdatedSuccessfully()
        {
            // Arrange
            int productId = 1;
            int quantity = 5;
            _mockTransactionService.Setup(service => service.UpdateProductinSaleAsync(productId, quantity)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProductsInSale(productId, quantity);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Product updated in sale", ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task UpdateProductsInSale_ShouldReturnBadRequest_WhenValidationExceptionThrown()
        {
            // Arrange
            int productId = 1;
            int quantity = 5;
            _mockTransactionService.Setup(service => service.UpdateProductinSaleAsync(productId, quantity)).ThrowsAsync(new ValidationException("This product is not in sale or invalid quantity"));

            // Act
            var result = await _controller.UpdateProductsInSale(productId, quantity);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("This product is not in sale or invalid quantity", ((BadRequestObjectResult)result).Value);
        }

     

        [Test]
        public async Task CalculateTotalAmount_ShouldReturnOk_WithTotalAmount()
        {
            // Arrange
            double totalAmount = 100.50;
            _mockTransactionService.Setup(service => service.CalculateTotalAmount()).ReturnsAsync(totalAmount);

            // Act
            var result = await _controller.CalculateTotalAmount();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(totalAmount, ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task CalculateTotalAmount_ShouldReturnNotFound_WhenNoProductsFound()
        {
            // Arrange
            _mockTransactionService.Setup(service => service.CalculateTotalAmount()).ReturnsAsync(0);

            // Act
            var result = await _controller.CalculateTotalAmount();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.AreEqual("No products found in sale", ((NotFoundObjectResult)result).Value);
        }
    }
}
