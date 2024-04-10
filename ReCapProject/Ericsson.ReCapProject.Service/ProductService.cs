using Ericsson.ReCapProject.Core.Attributes;
using Ericsson.ReCapProject.Core.Contracts.Persistence;
using Ericsson.ReCapProject.Core.Contracts.Service;
using Ericsson.ReCapProject.Core.Entitites;

namespace Ericsson.ReCapProject.Service
{
    [InjectableScoped]
    public class ProductService(IUnitOfWork unitOfWork) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.Products.GetAllProductsAsync();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _unitOfWork.Products.GetProductByIdAsync(productId);
        }

        public async Task CreateProductAsync(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name cannot be null or empty.");

            if (await _unitOfWork.Products.ExistsAsync(product.Name))
                throw new InvalidOperationException($"Product with name '{product.Name}' already exists.");

            await _unitOfWork.Products.CreateProductAsync(product);

            var isSuccessfullyCompleted = await _unitOfWork.CompleteAsync();
            if (!isSuccessfullyCompleted)
                throw new InvalidOperationException("Failed to create product.");
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await GetProductByIdAsync(product.Id);
            existingProduct.Update(product);

            var isSuccessfullyCompleted = await _unitOfWork.CompleteAsync();
            if (!isSuccessfullyCompleted)
                throw new InvalidOperationException("Failed to update product.");
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _unitOfWork.Products.DeleteProductAsync(productId);

            var isSuccessfullyCompleted = await _unitOfWork.CompleteAsync();
            if (!isSuccessfullyCompleted)
                throw new InvalidOperationException("Failed to delete product.");
        }

        public async Task AdjustStockQuantityAsync(int productId, int quantityChange)
        {
            Product product = await _unitOfWork.Products.GetProductByIdAsync(productId);
            product.AdjustStockQuantity(quantityChange);

            var isSuccessfullyCompleted = await _unitOfWork.CompleteAsync();
            if (!isSuccessfullyCompleted)
                throw new InvalidOperationException("Failed to adjust product.");
        }

        public async Task<decimal> CalculateTotalInventoryValueAsync()
        {
            var products = await _unitOfWork.Products.GetAllProductsAsync();
            return products.Sum(p => p.Price * p.StockQuantity);
        }

        public async Task<IEnumerable<Product>> GetHighValueProductsAsync(decimal thresholdValue)
        {
            return await _unitOfWork.Products.GetHighValueProductsAsync(thresholdValue);
        }
    }
}
