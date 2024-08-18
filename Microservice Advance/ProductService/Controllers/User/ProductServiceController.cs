using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductService.Models;

namespace ProductService.Controllers.User;

[Route("api/product")]
[ApiController]
public class ProductServiceController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly List<Product> _products = PredefinedProduct.Products;
    private readonly HttpClient _client = httpClientFactory.CreateClient("ProductDetailServiceClient");
    
    [HttpGet("get-all")]
    public IActionResult GetAllProducts()
    {
        try
        {
            return Ok(_products);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        try
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // Fetch product details from ProductDetails service
            var productDetails = await GetProductDetailsAsync(product.ProductId);
            var productWithDetails = new
            {
                product.ProductId,
                product.Name,
                product.Description,
                ProductDetails = productDetails
            };
            return Ok(productWithDetails);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }

    private async Task<List<ProductDetailDto>> GetProductDetailsAsync(int productId)
    {
        // Move the client to constructor level and address should point to api gateway
        try
        {
            var response = await _client.GetAsync($"get-all-by-product-id/{productId}"); // Adjust the URL as needed
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<ProductDetailDto>>(jsonString);
            return result ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }
}