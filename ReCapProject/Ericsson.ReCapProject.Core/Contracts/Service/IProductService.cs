using Ericsson.ReCapProject.Core.Entitites;

namespace Ericsson.ReCapProject.Core.Contracts.Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task CreateProductAsync(Product user);
        Task UpdateProductAsync(Product user);
        Task DeleteProductAsync(int productId);
        Task AdjustStockQuantityAsync(int productId, int quantityChange);
        Task<decimal> CalculateTotalInventoryValueAsync();
        Task<IEnumerable<Product>> GetHighValueProductsAsync(decimal thresholdValue);
    }
}
