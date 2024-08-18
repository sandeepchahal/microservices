using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models;

namespace ProductService.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("api/admin/product")]
[ApiController]
public class ProductServiceController(IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly List<Product> _products = PredefinedProduct.Products;
    readonly HttpClient _client = httpClientFactory.CreateClient("ProductDetailServiceClient");

    [HttpPost("add")]
    public IActionResult AddProduct([FromBody] Product product)
    {
        try
        {
            product.ProductId = _products.Count + 1;
            _products.Add(product);
            return StatusCode(StatusCodes.Status201Created, "New Product has been added successfully");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> RemoveProduct(int id)
    {
        try
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // remove from product details as well
            var response = await _client.GetAsync($"delete-by-product-id/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Unexpected error occurred while deleting the product");
            }

            _products.Remove(product);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }
}