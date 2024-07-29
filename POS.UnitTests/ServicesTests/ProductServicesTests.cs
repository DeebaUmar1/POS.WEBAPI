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
    public async Task ViewProductsAsync_ShouldDisplayProducts_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, name = "Product1", price = 10.0, quantity = 5, type = "Type1", category = "Category1" },
            new Product { Id = 2, name = "Product2", price = 20.0, quantity = 10, type = "Type2", category = "Category2" }
        };
        _productRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(products);

        using var consoleOutput = new ConsoleOutput(); // Custom helper class to capture console output
        await _productService.ViewProductsAsync();

        // Act
        var output = consoleOutput.GetOuput();

        // Assert
        Assert.That(output, Contains.Substring("Products List:"));
        Assert.That(output, Contains.Substring("Product1"));
        Assert.That(output, Contains.Substring("Product2"));
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
        var productId = 1;
        var existingProduct = new Product { Id = productId, name = "OldProduct", price = 10.0, quantity = 5, type = "OldType", category = "OldCategory" };
        var updatedProduct = new Product { name = "NewProduct", price = 20.0, quantity = 10, type = "NewType", category = "NewCategory" };
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.UpdateProductAsync(productId, updatedProduct);

        // Assert
        Assert.IsTrue(result); // Use IsTrue for boolean results
        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Product>(p => p.name == "NewProduct")), Times.Once);
    }
    
   


    [Test]
    public async Task RemoveProductAsync_ShouldReturnTrue_WhenProductIsRemovedSuccessfully()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product { Id = productId };
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(repo => repo.DeleteAsync(productId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.RemoveProductAsync(productId);

        // Assert
        Assert.IsTrue(result);
        _productRepositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);
    }
        
    [Test]
    public async Task UpdateStockAsync_ShouldReturnTrue_WhenStockIsUpdatedSuccessfully()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product { Id = productId, quantity = 10 };
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _productService.UpdateStockAsync(productId, 5, isIncrement: true);

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

// Custom helper class to capture console output
public class ConsoleOutput : IDisposable
{
    private readonly StringWriter _stringWriter;
    private readonly TextWriter _originalOutput;

    public ConsoleOutput()
    {
        _stringWriter = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_stringWriter);
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _stringWriter.Dispose();
    }

    public string GetOuput() => _stringWriter.ToString();
}
