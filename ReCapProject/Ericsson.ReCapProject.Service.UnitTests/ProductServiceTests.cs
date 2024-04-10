using Ericsson.ReCapProject.Core.Contracts.Persistence;
using Ericsson.ReCapProject.Core.Entitites;

namespace Ericsson.ReCapProject.Service.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var productService = new ProductService(unitOfWork);
            var products = new List<Product> { new(), new() };
            A.CallTo(() => unitOfWork.Products.GetAllProductsAsync())
                .Returns(Task.FromResult<IEnumerable<Product>>(products));

            // Act
            var result = await productService.GetAllProductsAsync();

            // Assert
            result.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingProduct_ShouldReturnProduct()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var productService = new ProductService(unitOfWork);
            var existingProduct = new Product();
            A.CallTo(() => unitOfWork.Products.GetProductByIdAsync(existingProduct.Id))
                .Returns(Task.FromResult(existingProduct));

            // Act
            var result = await productService.GetProductByIdAsync(existingProduct.Id);

            // Assert
            result.Should().Be(existingProduct);
            A.CallTo(() => unitOfWork.Products.GetProductByIdAsync(existingProduct.Id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_ShouldNotThrowException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            A.CallTo(() => unitOfWork.Products.ExistsAsync(A<string>._)).Returns(Task.FromResult(false));
            A.CallTo(() => unitOfWork.Products.CreateProductAsync(A<Product>._)).Returns(Task.CompletedTask);
            A.CallTo(() => unitOfWork.CompleteAsync()).Returns(Task.FromResult(true));

            var productService = new ProductService(unitOfWork);
            var product = new Product { Name = "NewProduct", Price = 20, StockQuantity = 3 };

            // Act
            Func<Task> act = async () => await productService.CreateProductAsync(product);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task CreateProductAsync_NullOrEmptyProductName_ShouldThrowArgumentException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var productService = new ProductService(unitOfWork);
            var product = new Product { Name = null, Price = 20, StockQuantity = 3 };

            // Act
            Func<Task> act = async () => await productService.CreateProductAsync(product);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Product name cannot be null or empty.");
        }

        [Fact]
        public async Task CreateProductAsync_DuplicateProductName_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            A.CallTo(() => unitOfWork.Products.ExistsAsync(A<string>._)).Returns(Task.FromResult(true));

            var productService = new ProductService(unitOfWork);
            var product = new Product { Name = "ExistingProduct", Price = 20, StockQuantity = 3 };

            // Act
            Func<Task> act = async () => await productService.CreateProductAsync(product);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Product with name 'ExistingProduct' already exists.");
        }

        [Fact]
        public async Task CreateProductAsync_FailedToCreateProduct_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            A.CallTo(() => unitOfWork.Products.ExistsAsync(A<string>._)).Returns(Task.FromResult(false));
            A.CallTo(() => unitOfWork.Products.CreateProductAsync(A<Product>._)).Returns(Task.CompletedTask);
            A.CallTo(() => unitOfWork.CompleteAsync()).Returns(Task.FromResult(false));

            var productService = new ProductService(unitOfWork);
            var product = new Product { Name = "NewProduct", Price = 20, StockQuantity = 3 };

            // Act
            Func<Task> act = async () => await productService.CreateProductAsync(product);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed to create product.");
        }

        [Fact]
        public async Task UpdateProductAsync_ValidProduct_ShouldNotThrowException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var existingProduct = new Product { Id = 1, Name = "ExistingProduct", Price = 20, StockQuantity = 3 };
            A.CallTo(() => unitOfWork.Products.GetProductByIdAsync(existingProduct.Id)).Returns(existingProduct);
            A.CallTo(() => unitOfWork.CompleteAsync()).Returns(Task.FromResult(true));

            var productService = new ProductService(unitOfWork);
            var updatedProduct = new Product { Id = 1, Name = "UpdatedProduct", Price = 30, StockQuantity = 5 };

            // Act
            Func<Task> act = async () => await productService.UpdateProductAsync(updatedProduct);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task UpdateProductAsync_FailedToUpdateProduct_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var existingProduct = new Product { Id = 1, Name = "ExistingProduct", Price = 20, StockQuantity = 3 };
            A.CallTo(() => unitOfWork.Products.GetProductByIdAsync(existingProduct.Id)).Returns(existingProduct);
            A.CallTo(() => unitOfWork.CompleteAsync()).Returns(Task.FromResult(false));

            var productService = new ProductService(unitOfWork);
            var updatedProduct = new Product { Id = 1, Name = "UpdatedProduct", Price = 30, StockQuantity = 5 };

            // Act
            Func<Task> act = async () => await productService.UpdateProductAsync(updatedProduct);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed to update product.");
        }

        [Fact]
        public async Task DeleteProductAsync_ValidProductId_ShouldNotThrowException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            A.CallTo(() => unitOfWork.Products.DeleteProductAsync(1)).Returns(Task.CompletedTask);
            A.CallTo(() => unitOfWork.CompleteAsync()).Returns(Task.FromResult(true));

            var productService = new ProductService(unitOfWork);

            // Act
            Func<Task> act = async () => await productService.DeleteProductAsync(1);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteProductAsync_FailedToDeleteProduct_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            A.CallTo(() => unitOfWork.Products.DeleteProductAsync(1)).Returns(Task.CompletedTask);
            A.CallTo(() => unitOfWork.CompleteAsync()).Returns(Task.FromResult(false));

            var productService = new ProductService(unitOfWork);

            // Act
            Func<Task> act = async () => await productService.DeleteProductAsync(1);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed to delete product.");
        }

        [Fact]
        public async Task AdjustStockQuantityAsync_ValidProductIdAndQuantityChange_ShouldNotThrowException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var existingProduct = new Product { Id = 1, Name = "ExistingProduct", Price = 20, StockQuantity = 3 };
            A.CallTo(() => unitOfWork.Products.GetProductByIdAsync(existingProduct.Id)).Returns(existingProduct);
            A.CallTo(() => unitOfWork.CompleteAsync()).Returns(Task.FromResult(true));

            var productService = new ProductService(unitOfWork);

            // Act
            Func<Task> act = async () => await productService.AdjustStockQuantityAsync(1, 5);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task AdjustStockQuantityAsync_FailedToAdjustStockQuantity_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var existingProduct = new Product { Id = 1, Name = "ExistingProduct", Price = 20, StockQuantity = 3 };
            A.CallTo(() => unitOfWork.Products.GetProductByIdAsync(existingProduct.Id)).Returns(existingProduct);
            A.CallTo(() => unitOfWork.CompleteAsync()).Returns(Task.FromResult(false));

            var productService = new ProductService(unitOfWork);

            // Act
            Func<Task> act = async () => await productService.AdjustStockQuantityAsync(1, 5);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed to adjust product.");
        }

        [Fact]
        public async Task CalculateTotalInventoryValueAsync_ShouldReturnTotalValue()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var productService = new ProductService(unitOfWork);
            var products = new List<Product>
        {
            new() { Price = 10.5m, StockQuantity = 3 },
            new() { Price = 5.25m, StockQuantity = 2 }
        };
            A.CallTo(() => unitOfWork.Products.GetAllProductsAsync()).Returns(products);

            // Act
            var result = await productService.CalculateTotalInventoryValueAsync();

            // Assert
            result.Should().Be(10.5m * 3 + 5.25m * 2); // (Price * StockQuantity) for each product
        }

        [Fact]
        public async Task GetHighValueProductsAsync_ShouldReturnHighValueProducts()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var productService = new ProductService(unitOfWork);
            var thresholdValue = 50m;
            var highValueProducts = new List<Product>
            {
                new() { Price = 30m, StockQuantity = 2 },
                new() { Price = 25m, StockQuantity = 3 }
            };
            A.CallTo(() => unitOfWork.Products.GetHighValueProductsAsync(thresholdValue)).Returns(highValueProducts);

            // Act
            var result = await productService.GetHighValueProductsAsync(thresholdValue);

            // Assert
            result.Should().BeEquivalentTo(highValueProducts);
        }

        [Fact]
        public async Task GetHighValueProductsAsync_ShouldReturnEmptyListWhenNoHighValueProducts()
        {
            // Arrange
            var unitOfWork = A.Fake<IUnitOfWork>();
            var productService = new ProductService(unitOfWork);
            var thresholdValue = 100m; // Set a high threshold to ensure no high-value products are returned
            A.CallTo(() => unitOfWork.Products.GetHighValueProductsAsync(thresholdValue)).Returns(new List<Product>());

            // Act
            var result = await productService.GetHighValueProductsAsync(thresholdValue);

            // Assert
            result.Should().BeEmpty();
        }
    }
}