using Microsoft.Azure.Cosmos;
using System.Net;
using System.Linq.Expressions;

namespace EcommerceDemo.Services.Products;

public class ProductRepository
{
    private readonly Container _container;

    public ProductRepository(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        _container = cosmosClient.GetContainer(databaseName, containerName);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Product>(
                id.ToString(),
                new PartitionKey(id.ToString())
            );
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var query = _container.GetItemQueryIterator<Product>("SELECT * FROM c");
        var results = new List<Product>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        var response = await _container.CreateItemAsync(
            product,
            new PartitionKey(product.Id.ToString())
        );
        return response.Resource;
    }

    public async Task CreateRangeAsync(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            await CreateAsync(product);
        }
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var response = await _container.UpsertItemAsync(
            product,
            new PartitionKey(product.Id.ToString())
        );
        return response.Resource;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            await _container.DeleteItemAsync<Product>(
                id.ToString(),
                new PartitionKey(id.ToString())
            );
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<int> GetHighestProductIdAsync()
    {
        var query = _container.GetItemQueryIterator<dynamic>(
            "SELECT MAX(c.productId) as maxId FROM c"
        );

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            var result = response.FirstOrDefault();
            return result != null ? result.maxId : 0;
        }

        return 0;
    }

    // public async Task<IEnumerable<Product>> GetItemsAsync(
    //     Expression<Func<Product, bool>> predicate)
    // {
    //     var query = _container.GetItemLinqQueryable<Product>()
    //         .Where(predicate)
    //         .ToFeedIterator();

    //     var results = new List<Product>();
    //     while (query.HasMoreResults)
    //     {
    //         var response = await query.ReadNextAsync();
    //         results.AddRange(response);
    //     }

    //     return results;
    // }
} 