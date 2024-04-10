using AutoMapper;
using Ericsson.ReCapProject.Api.ViewModels;
using Ericsson.ReCapProject.Core.Contracts.Service;
using Ericsson.ReCapProject.Core.Entitites;
using Microsoft.AspNetCore.Mvc;

namespace Ericsson.ReCapProject.Api.Controllers
{
    public class ProductController(IProductService productService, IMapper mapper, ILogger<ProductController> logger) : BaseController
    {
        private readonly IProductService _productService = productService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var products = await _productService.GetAllProductsAsync();
            var productViewModels = _mapper.Map<IEnumerable<ProductViewModel>>(products);
            _logger.LogWarning("Test Log for Application Insights will be removed.");
            return Ok(productViewModels);
        }

        [HttpGet("{productId}")]
        [ActionName(nameof(GetProductByIdAsync))]
        public async Task<IActionResult> GetProductByIdAsync(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            var productViewModel = _mapper.Map<ProductViewModel>(product);
            return Ok(productViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(ProductViewModel productViewModel)
        {
            var product = _mapper.Map<Product>(productViewModel);
            await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProductByIdAsync), new { productId = product.Id }, productViewModel);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProductAsync(ProductViewModel productViewModel)
        {
            var product = _mapper.Map<Product>(productViewModel);
            await _productService.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            await _productService.DeleteProductAsync(productId);
            return NoContent();
        }

        [HttpPatch("{productId}/adjust-stock/{quantityChange}")]
        public async Task<IActionResult> AdjustStockQuantityAsync(int productId, int quantityChange)
        {
            await _productService.AdjustStockQuantityAsync(productId, quantityChange);
            return NoContent();
        }

        [HttpGet("total-inventory-value")]
        public async Task<IActionResult> CalculateTotalInventoryValueAsync()
        {
            decimal totalValue = await _productService.CalculateTotalInventoryValueAsync();
            return Ok(totalValue);
        }
    }
}