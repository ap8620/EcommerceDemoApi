using Newtonsoft.Json;

namespace EcommerceDemo.Services.Products;

public class Product
{
    [JsonProperty("id")]
    public string? CosmosId 
    { 
        get => Id.ToString(); 
        set => Id = value != null ? int.Parse(value) : 0; 
    }

    [JsonProperty("productId")]
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public Rating Rating { get; set; } = new Rating();
}

public class Rating
{
    public decimal Rate { get; set; }
    public int Count { get; set; }
} 