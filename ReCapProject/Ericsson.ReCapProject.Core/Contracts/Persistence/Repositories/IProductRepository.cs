using Ericsson.ReCapProject.Core.Entitites;

namespace Ericsson.ReCapProject.Core.Contracts.Persistence.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task CreateProductAsync(Product user);
        Task UpdateProductAsync(Product user);
        Task DeleteProductAsync(int productId);
        Task<bool> ExistsAsync(string productName);
        Task<IEnumerable<Product>> GetHighValueProductsAsync(decimal thresholdValue);
    }
}