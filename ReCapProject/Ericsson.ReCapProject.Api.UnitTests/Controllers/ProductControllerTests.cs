using Ericsson.ReCapProject.Api.Controllers;
using Ericsson.ReCapProject.Api.ViewModels;
using Ericsson.ReCapProject.Core.Contracts.Persistence;
using Ericsson.ReCapProject.Core.Contracts.Service;
using Ericsson.ReCapProject.Core.Entitites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ericsson.ReCapProject.Api.UnitTests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Fixture _fixture;
        private readonly IMapper _mapper;

        public ProductControllerTests()
        {
            _fixture = new Fixture();
            _mapper = A.Fake<IMapper>();
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnOkWithProductViewModels()
        {
            // Arrange
            var productService = A.Fake<IProductService>();
            var mapper = A.Fake<IMapper>();
            var logger = A.Fake<ILogger<ProductController>>();
            var productController = new ProductController(productService, mapper, logger);
            var products = _fixture.CreateMany<Product>(3);
            var productViewModels = _fixture.CreateMany<ProductViewModel>(3);
            A.CallTo(() => productService.GetAllProductsAsync()).Returns(products);
            A.CallTo(() => mapper.Map<IEnumerable<ProductViewModel>>(products)).Returns(productViewModels);

            // Act
            var result = await productController.GetAllProductsAsync() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(productViewModels);
        }

        [Fact]
        public async Task GetProductByIdAsync_ExistingProduct_ShouldReturnOkWithProductViewModel()
        {
            // Arrange
            var productService = A.Fake<IProductService>();
            var mapper = A.Fake<IMapper>();
            var logger = A.Fake<ILogger<ProductController>>();
            var productController = new ProductController(productService, mapper, logger);
            var existingProduct = _fixture.Create<Product>();
            var productViewModel = _fixture.Create<ProductViewModel>();
            A.CallTo(() => productService.GetProductByIdAsync(existingProduct.Id)).Returns(existingProduct);
            A.CallTo(() => mapper.Map<ProductViewModel>(existingProduct)).Returns(productViewModel);

            // Act
            var result = await productController.GetProductByIdAsync(existingProduct.Id) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(productViewModel);
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var productService = A.Fake<IProductService>();
            var mapper = A.Fake<IMapper>();
            var unitOfWork = A.Fake<IUnitOfWork>();
            var logger = A.Fake<ILogger<ProductController>>();
            var productController = new ProductController(productService, mapper, logger);
            var productViewModel = new ProductViewModel();
            var product = new Product();
            A.CallTo(() => mapper.Map<Product>(productViewModel)).Returns(product);
            A.CallTo(() => productService.CreateProductAsync(product)).Invokes(() => unitOfWork.CompleteAsync());

            // Act
            var result = await productController.CreateProductAsync(productViewModel) as CreatedAtActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(ProductController.GetProductByIdAsync));
            result.RouteValues.Should().ContainKey("productId");
            result.RouteValues["productId"].Should().Be(product.Id);
            result.Value.Should().BeEquivalentTo(productViewModel);
        }

        [Fact]
        public async Task UpdateProductAsync_ValidProduct_ShouldReturnNoContent()
        {
            // Arrange
            var productService = A.Fake<IProductService>();
            var mapper = A.Fake<IMapper>();
            var unitOfWork = A.Fake<IUnitOfWork>();
            var logger = A.Fake<ILogger<ProductController>>();
            var productController = new ProductController(productService, mapper, logger);
            var productViewModel = new ProductViewModel();
            var product = new Product();
            A.CallTo(() => mapper.Map<Product>(productViewModel)).Returns(product);
            A.CallTo(() => productService.UpdateProductAsync(product)).Invokes(() => unitOfWork.CompleteAsync());

            // Act
            var result = await productController.UpdateProductAsync(productViewModel) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_ShouldReturnNoContent()
        {
            // Arrange
            var productService = A.Fake<IProductService>();
            var mapper = A.Fake<IMapper>();
            var unitOfWork = A.Fake<IUnitOfWork>();
            var logger = A.Fake<ILogger<ProductController>>();
            var productController = new ProductController(productService, mapper, logger);
            A.CallTo(() => productService.DeleteProductAsync(1)).Invokes(() => unitOfWork.CompleteAsync());

            // Act
            var result = await productController.DeleteProductAsync(1) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task AdjustStockQuantityAsync_ValidProduct_ShouldReturnNoContent()
        {
            // Arrange
            var productService = A.Fake<IProductService>();
            var mapper = A.Fake<IMapper>();
            var unitOfWork = A.Fake<IUnitOfWork>();
            var logger = A.Fake<ILogger<ProductController>>();
            var productController = new ProductController(productService, mapper, logger);
            A.CallTo(() => productService.AdjustStockQuantityAsync(1, 5)).Invokes(() => unitOfWork.CompleteAsync());

            // Act
            var result = await productController.AdjustStockQuantityAsync(1, 5) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CalculateTotalInventoryValueAsync_ShouldReturnOkWithDecimalValue()
        {
            // Arrange
            var productService = A.Fake<IProductService>();
            var mapper = A.Fake<IMapper>();
            var logger = A.Fake<ILogger<ProductController>>();
            var productController = new ProductController(productService, mapper, logger);
            A.CallTo(() => productService.CalculateTotalInventoryValueAsync()).Returns(100.5m);

            // Act
            var result = await productController.CalculateTotalInventoryValueAsync() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.Value.Should().Be(100.5m);
        }
    }
}