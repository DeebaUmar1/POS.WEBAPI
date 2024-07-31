using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using POS.Data;
using POS.Models.Entities;
using POS.Repositories.ProductRepository;
using POS.Repositories.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class ProductRepositoryTests
{
    private POSDbContext _context;
    private ProductRepository _repository;

    [SetUp]
    public void Setup()
    {
        // Create a new instance of the in-memory database context
        var options = new DbContextOptionsBuilder<POSDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new POSDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product { id = "4", name = "TestProduct", price = 100.0, quantity = 10, type = "TestType", category = "TestCategory" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(4);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(product.name, result.name);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { id = "1", name = "Product1", price = 10.0, quantity = 10, type = "Type1", category = "Category1" },
            new Product { id = "2", name = "Product2", price = 20.0, quantity = 20, type = "Type2", category = "Category2" }
        };
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Product1", result[0].name);
        Assert.AreEqual("Product2", result[1].name);
    }

    [Test]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var product = new Product { id = "3", name = "Product3", price = 30.0, quantity = 30, type = "Type3", category = "Category3" };

        // Act
        await _repository.AddAsync(product);

        // Assert
        var addedProduct = await _repository.GetByIdAsync(3);
        Assert.IsNotNull(addedProduct);
        Assert.AreEqual(product.name, addedProduct.name);
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product { id = "4",  name = "Product4", price = 40.0, quantity = 40, type = "Type4", category = "Category4" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        product.price = 45.0;

        // Act
        await _repository.UpdateAsync(product);

        // Assert
        var updatedProduct = await _repository.GetByIdAsync(4);
        Assert.AreEqual(45.0, updatedProduct.price);
    }

    [Test]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        // Arrange
        var product = new Product { id = "5", name = "Product5", price = 50.0, quantity = 50, type = "Type5", category = "Category5" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(5);

        // Assert
        var deletedProduct = await _repository.GetByIdAsync(5);
        Assert.IsNull(deletedProduct);
    }

    [Test]
    public async Task SeedProducts_ShouldAddInitialProducts_WhenNoProductsExist()
    {
        // Arrange
        // Ensure database is empty
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();

        // Act
        await _repository.SeedProducts();

        // Assert
        var products = await _repository.GetAllAsync();
        Assert.AreEqual(3, products.Count);
        Assert.IsTrue(products.Any(p => p.name == "Laptop"));
        Assert.IsTrue(products.Any(p => p.name == "Mouse"));
        Assert.IsTrue(products.Any(p => p.name == "Keyboard"));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
