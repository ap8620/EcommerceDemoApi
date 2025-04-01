using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using EcommerceDemo.Services.Products;

namespace EcommerceDemo.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEcommerceServices(
        this IServiceCollection services, 
        string connectionString)
    {
        services.AddSingleton(sp => 
        {
            var client = new CosmosClient(connectionString);
            
            // Ensure database and container exist
            var database = client.CreateDatabaseIfNotExistsAsync("EcommerceDemo")
                .GetAwaiter()
                .GetResult();
                
            database.Database.CreateContainerIfNotExistsAsync(
                "products",
                "/id",
                400) // Set your desired RU/s
                .GetAwaiter()
                .GetResult();
                
            return client;
        });

        services.AddScoped<Products.IProductService, ProductService>();
        return services;
    }
} 