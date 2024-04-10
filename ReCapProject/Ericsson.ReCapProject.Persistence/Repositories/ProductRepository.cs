using Ericsson.ReCapProject.Core.Attributes;
using Ericsson.ReCapProject.Core.Contracts.Persistence.Repositories;
using Ericsson.ReCapProject.Core.Entitites;
using Ericsson.ReCapProject.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Ericsson.ReCapProject.Persistence.Repositories
{
    [InjectableScoped]
    public class ProductRepository(ReCapProjectDbContext dbContext) : IProductRepository
    {
        private readonly ReCapProjectDbContext _ReCapProjectDbContext = dbContext;

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _ReCapProjectDbContext.Products
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var product = await _ReCapProjectDbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            return product ?? throw new KeyNotFoundException($"Product with ID {productId} not found.");
        }

        public async Task CreateProductAsync(Product product)
        {
            await _ReCapProjectDbContext.Products.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            var existingProduct = await GetProductByIdAsync(product.Id);
            existingProduct.Update(product);
        }

        public async Task DeleteProductAsync(int productId)
        {
            var existingProduct = await GetProductByIdAsync(productId);
            _ReCapProjectDbContext.Products.Remove(existingProduct);
        }

        public async Task<bool> ExistsAsync(string productName)
        {
            return await _ReCapProjectDbContext.Products.AnyAsync(p => p.Name == productName);
        }

        public async Task<IEnumerable<Product>> GetHighValueProductsAsync(decimal thresholdValue)
        {
            return await _ReCapProjectDbContext.Products
                .AsQueryable()
                .Where(p => p.Price * p.StockQuantity > thresholdValue)
                .ToListAsync();
        }
    }
}
