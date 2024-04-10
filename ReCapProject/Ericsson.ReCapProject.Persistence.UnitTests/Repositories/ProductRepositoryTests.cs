using Ericsson.ReCapProject.Core.Entitites;
using Ericsson.ReCapProject.Persistence;
using Ericsson.ReCapProject.Persistence.DbContexts;
using Ericsson.ReCapProject.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ericsson.ReCapProject.Repository.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_GetAllProductsAsync")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                var productRepository = new ProductRepository(context);

                // Seed the in-memory database with test data
                context.Products.AddRange(new List<Product>
                {
                    new() { Id = 1, Name = "Product1", Price = 10, StockQuantity = 5 },
                    new() { Id = 2, Name = "Product2", Price = 15, StockQuantity = 8 }
                });
                await context.SaveChangesAsync();

                // Act
                var result = await productRepository.GetAllProductsAsync();

                // Assert
                result.Should().HaveCount(2);
            }
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingProduct_ShouldReturnProduct()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_GetProductByIdAsync")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                var productRepository = new ProductRepository(context);

                // Seed the in-memory database with test data
                context.Products.AddRange(new List<Product>
                {
                    new() { Id = 1, Name = "Product1", Price = 10, StockQuantity = 5 },
                    new() { Id = 2, Name = "Product2", Price = 15, StockQuantity = 8 }
                });
                await context.SaveChangesAsync();

                // Act
                var result = await productRepository.GetProductByIdAsync(1);

                // Assert
                result.Should().NotBeNull();
                result.Id.Should().Be(1);
                result.Name.Should().Be("Product1");
            }
        }

        [Fact]
        public async Task GetProductByIdAsync_NonExistingProduct_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_GetProductByIdAsync")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                var productRepository = new ProductRepository(context);

                // Act
                Func<Task> act = async () => await productRepository.GetProductByIdAsync(3);

                // Assert
                await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Product with ID 3 not found.");
            }
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddProductToDatabase()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_CreateProductAsync")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                var productRepository = new ProductRepository(context);
                var newProduct = new Product { Name = "NewProduct", Price = 20, StockQuantity = 3 };

                // Act
                Func<Task> act = async () => await productRepository.CreateProductAsync(newProduct);

                // Assert
                await act.Should().NotThrowAsync();
            }
        }

        [Fact]
        public async Task UpdateProductAsync_ExistingProduct_ShouldUpdateProduct()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_UpdateProductAsync")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                // Seed the in-memory database with test data
                context.Products.AddRange(new List<Product>
                {
                    new() { Id = 1, Name = "Product1", Price = 10, StockQuantity = 5 },
                    new() { Id = 2, Name = "Product2", Price = 15, StockQuantity = 8 }
                });
                await context.SaveChangesAsync();

                var productRepository = new ProductRepository(context);
                var updatedProduct = new Product { Id = 1, Name = "UpdatedProduct", Price = 12, StockQuantity = 7 };

                // Act
                Func<Task> act = async () => await productRepository.UpdateProductAsync(updatedProduct);

                // Assert
                await act.Should().NotThrowAsync();
            }
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_ShouldRemoveProductFromDatabase()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_DeleteProductAsync")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                // Seed the in-memory database with test data
                context.Products.AddRange(new List<Product>
                {
                    new() { Id = 1, Name = "Product1", Price = 10, StockQuantity = 5 },
                    new() { Id = 2, Name = "Product2", Price = 15, StockQuantity = 8 }
                });
                await context.SaveChangesAsync();
            }

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                var productRepository = new ProductRepository(context);

                // Act
                Func<Task> act = async () => await productRepository.DeleteProductAsync(1);

                // Assert
                await act.Should().NotThrowAsync();
            }
        }

        [Fact]
        public async Task ExistsAsync_ProductNameExists_ShouldReturnTrue()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_ExistsAsync_ProductNameExists")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                var productRepository = new ProductRepository(context);
                var unitOfWork = new UnitOfWork(context);
                var productName = "ExistingProduct";
                var products = new List<Product>
                {
                    new() { Id = 1, Name = "Product1", Price = 10, StockQuantity = 5 },
                    new() { Id = 2, Name = productName, Price = 15, StockQuantity = 8 }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                // Act
                var result = await productRepository.ExistsAsync(productName);

                // Assert
                result.Should().BeTrue();
            }
        }

        [Fact]
        public async Task ExistsAsync_ProductNameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_ExistsAsync_ProductNameDoesNotExist")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                var productRepository = new ProductRepository(context);
                var unitOfWork = new UnitOfWork(context);
                var productName = "NonExistingProduct";
                var products = new List<Product>
                {
                    new() { Id = 1, Name = "Product1", Price = 10, StockQuantity = 5 },
                    new() { Id = 2, Name = "Product2", Price = 15, StockQuantity = 8 }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                // Act
                var result = await productRepository.ExistsAsync(productName);

                // Assert
                result.Should().BeFalse();
            }
        }

        [Fact]
        public async Task GetHighValueProductsAsync_ShouldReturnHighValueProducts()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<ReCapProjectDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_GetHighValueProductsAsync")
                .Options;

            using (var context = new ReCapProjectDbContext(dbContextOptions))
            {
                var productRepository = new ProductRepository(context);

                // Seed the in-memory database with test data
                context.Products.AddRange(new List<Product>
                {
                    new() { Id = 1, Name = "Product1", Price = 30, StockQuantity = 2 },
                    new() { Id = 2, Name = "Product2", Price = 25, StockQuantity = 3 }
                });
                await context.SaveChangesAsync();

                // Act
                var result = await productRepository.GetHighValueProductsAsync(50);

                // Assert
                result.Should().HaveCount(2);
            }
        }
    }
}