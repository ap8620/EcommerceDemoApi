using Microsoft.Azure.Cosmos;
using System.Net;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EcommerceDemo.Services.Products;

public interface IProductService
{
    Task<Product> CreateProductAsync(Product product);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<bool> SeedProductsAsync(List<Product> products);
    Task<Product?> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
}

public class ProductService : IProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(CosmosClient cosmosClient)
    {
        _productRepository = new ProductRepository(cosmosClient, "EcommerceDemo", "products");
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        // Get all products from repository
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<bool> SeedProductsAsync(List<Product> products)
    {
        // Using CreateRangeAsync instead of SeedAsync since that's the standard Cosmos repository method
        await _productRepository.CreateRangeAsync(products);
        return true;
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        // If ID is not set (equals 0), generate a new one
        if (product.Id == 0)
        {
            var highestId = await _productRepository.GetHighestProductIdAsync();
            product.Id = highestId + 1;
        }
        
        return await _productRepository.CreateAsync(product);
    }

    public async Task<Product?> UpdateProductAsync(Product product)
    {
        // Check if product exists
        var existingProduct = await _productRepository.GetByIdAsync(product.Id);
        if (existingProduct == null)
        {
            return null;
        }
        
        return await _productRepository.UpdateAsync(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }
}