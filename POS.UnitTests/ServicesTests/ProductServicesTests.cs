using Moq;
using NUnit.Framework;
using POS.Models.Entities;
using POS.Repositories.ProductRepository;
using POS.Services.ProductServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class ProductServiceTests
{
    private Mock<IProductRepository> _productRepositoryMock;
    private ProductService _productService;

    [SetUp]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productService = new ProductService(_productRepositoryMock.Object);
    }

    

    [Test]
    public async Task AddProductAsync_ShouldReturnTrue_WhenProductIsValid()
    {
        // Arrange
        var product = new Product { name = "NewProduct", price = 10.0, quantity = 5, type = "Type", category = "Category" };
        _productRepositoryMock.Setup(repo => repo.AddAsync(product))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.AddProductAsync(product);

      

        // Assert
        Assert.IsTrue(result);
       
    }

    [Test]
    public async Task AddProductAsync_ShouldReturnFalse_WhenProductIsInvalid()
    {
        // Arrange
        var product = new Product { name = "", price = 10.0, quantity = 5, type = "Type", category = "Category" };

        // Act
        var result = await _productService.AddProductAsync(product);

        // Assert
        Assert.IsFalse(result);
        _productRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Never);
    }
    [Test]
    public async Task UpdateProductAsync_ShouldReturnTrue_WhenProductIsUpdatedSuccessfully()
    {
        // Arrange
        var productId = "1";
        var existingProduct = new Product { id = productId, name = "OldProduct", price = 10.0, quantity = 5, type = "OldType", category = "OldCategory" };
        var updatedProduct = new Product { name = "NewProduct", price = 20.0, quantity = 10, type = "NewType", category = "NewCategory" };
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(Convert.ToInt32(productId)))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.UpdateProductAsync(Convert.ToInt32(productId), updatedProduct);

        // Assert
        Assert.IsTrue(result); // Use IsTrue for boolean results
        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Product>(p => p.name == "NewProduct")), Times.Once);
    }
    
   


    [Test]
    public async Task RemoveProductAsync_ShouldReturnTrue_WhenProductIsRemovedSuccessfully()
    {
        // Arrange
        var productId = "1";
        var existingProduct = new Product { id = productId };
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(Convert.ToInt32(productId)))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(repo => repo.DeleteAsync(Convert.ToInt32(productId)))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.RemoveProductAsync(Convert.ToInt32(productId));

        // Assert
        Assert.IsTrue(result);
        _productRepositoryMock.Verify(repo => repo.DeleteAsync(Convert.ToInt32(productId)), Times.Once);
    }
        
    [Test]
    public async Task UpdateStockAsync_ShouldReturnTrue_WhenStockIsUpdatedSuccessfully()
    {
        // Arrange
        var productId = "1";
        var existingProduct = new Product { id = productId, quantity = 10 };
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(Convert.ToInt32(productId)))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.UpdateStockAsync(Convert.ToInt32(productId), 5, isIncrement: true);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(15, existingProduct.quantity); // Ensure the stock is updated correctly
    }

    [Test]
    public async Task SeedProducts_ShouldCallSeedProductsOnRepository()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.SeedProducts())
            .Returns(Task.CompletedTask);

        // Act
        await _productService.SeedProducts();

        // Assert
        _productRepositoryMock.Verify(repo => repo.SeedProducts(), Times.Once);
    }
}
