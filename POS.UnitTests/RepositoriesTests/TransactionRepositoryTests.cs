using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using POS.Data;
using POS.Models.Entities;
using POS.Repositories.TransactionRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class TransactionRepositoryTests
{
    private POSDbContext _context;
    private TransactionRepository _repository;

    [SetUp]
    public void Setup()
    {
        // Create a new instance of the in-memory database context
        var options = new DbContextOptionsBuilder<POSDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new POSDbContext(options);
        _repository = new TransactionRepository(_context);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnSaleProduct_WhenProductExists()
    {
        // Arrange
        var saleProduct = new SaleProducts { SalesTransactionId = 1, Quantity = 5, ProductId = 1, ProductName = "Product1", ProductPrice = 10.0 };
        _context.SaleProducts.Add(saleProduct);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(saleProduct.ProductName, result.ProductName);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllSaleProducts()
    {
        // Arrange
        var saleProducts = new List<SaleProducts>
        {
            new SaleProducts { SalesTransactionId = 1, Quantity = 2, ProductId = 1, ProductName = "Product1", ProductPrice = 10.0 },
            new SaleProducts { SalesTransactionId = 2, Quantity = 3, ProductId = 2, ProductName = "Product2", ProductPrice = 20.0 }
        };
        _context.SaleProducts.AddRange(saleProducts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Product1", result[0].ProductName);
        Assert.AreEqual("Product2", result[1].ProductName);
    }

    [Test]
    public async Task AddAsync_ShouldAddSaleProduct()
    {
        // Arrange
        var saleProduct = new SaleProducts { SalesTransactionId = 3, Quantity = 4, ProductId = 3, ProductName = "Product3", ProductPrice = 30.0 };

        // Act
        await _repository.AddAsync(saleProduct);

        // Assert
        var addedProduct = await _repository.GetByIdAsync(3);
        Assert.IsNotNull(addedProduct);
        Assert.AreEqual(saleProduct.ProductName, addedProduct.ProductName);
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateSaleProduct()
    {
        // Arrange
        var saleProduct = new SaleProducts { SalesTransactionId = 4, Quantity = 5, ProductId = 4, ProductName = "Product4", ProductPrice = 40.0 };
        _context.SaleProducts.Add(saleProduct);
        await _context.SaveChangesAsync();

        saleProduct.ProductPrice = 45.0;

        // Act
        await _repository.UpdateAsync(saleProduct);

        // Assert
        var updatedProduct = await _repository.GetByIdAsync(4);
        Assert.AreEqual(45.0, updatedProduct.ProductPrice);
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveSaleProduct()
    {
        // Arrange
        var saleProduct = new SaleProducts { SalesTransactionId = 5, Quantity = 6, ProductId = 5, ProductName = "Product5", ProductPrice = 50.0 };
        _context.SaleProducts.Add(saleProduct);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(5);

        // Assert
        var deletedProduct = await _repository.GetByIdAsync(5);
        Assert.IsNull(deletedProduct);
    }

    [Test]
    public async Task RemoveAll_ShouldRemoveAllSaleProducts()
    {
        // Arrange
        var saleProducts = new List<SaleProducts>
        {
            new SaleProducts { SalesTransactionId = 6, Quantity = 7, ProductId = 6, ProductName = "Product6", ProductPrice = 60.0 },
            new SaleProducts { SalesTransactionId = 7, Quantity = 8, ProductId = 7, ProductName = "Product7", ProductPrice = 70.0 }
        };
        _context.SaleProducts.AddRange(saleProducts);
        await _context.SaveChangesAsync();

        // Act
        _repository.RemoveAll(saleProducts);

        // Assert
        var products = await _repository.GetAllAsync();
        Assert.AreEqual(0, products.Count);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
