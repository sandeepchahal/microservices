using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDetailService.Models;

namespace ProductDetailService.Controllers.Admin;

[Route("api/admin/product/detail")]
[Authorize(Roles = "Admin")]
[ApiController]
public class ProductDetailServiceController : ControllerBase
{
    private static readonly List<ProductDetail> ProductDetails = PredefinedProductDetail.GetProductDetails.ToList();

    [HttpPost("add/{productId}")]
    public IActionResult AddProductDetail(int productId, [FromBody] ProductDetail productDetail)
    {
        try
        {
            if (ProductDetails.FirstOrDefault(col => col.ProductId == productId) == null)
                return NotFound($"Product Id {productId} is not found");

            productDetail.ProductDetailId = ProductDetails.Count + 1;
            ProductDetails.Add(productDetail);
            return StatusCode(StatusCodes.Status201Created, "New Product Detail is added successfully");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }

    [HttpDelete("delete/{id}")]
    public IActionResult RemoveProductDetail(int id)
    {
        try
        {
            var productDetail = ProductDetails.FirstOrDefault(pd => pd.ProductDetailId == id);
            if (productDetail == null)
            {
                return NotFound();
            }

            ProductDetails.Remove(productDetail);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }
    
    [HttpDelete("delete-by-product-id/{id}")]
    public IActionResult RemoveProductRangeDetail(int id)
    {
        try
        {
            var productDetail = ProductDetails.FirstOrDefault(pd => pd.ProductId == id);
            if (productDetail == null)
            {
                return NotFound();
            }

            ProductDetails.RemoveAll(col => col.ProductId == id);
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }
    
}