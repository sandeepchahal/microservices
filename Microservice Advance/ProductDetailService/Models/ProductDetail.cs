namespace ProductDetailService.Models;

public class ProductDetail
{
    public int ProductDetailId { get; set; }
    public int ProductId { get; set; }
    public string Size { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Design { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class ProductQuantityContext
{
    public int ProductDetailId { get; set; }
    public int Quantity { get; set; }
}
public static class PredefinedProductDetail
{
    public static IEnumerable<ProductDetail> GetProductDetails => new[]
    {
        new ProductDetail(){ProductDetailId = 1, ProductId = 1, Design = "Design 11", Price = 1100, Size = "A1", Quantity = 10},
        new ProductDetail(){ProductDetailId = 2, ProductId = 1, Design = "Design 12", Price = 1200, Size = "A2",Quantity = 20},
        new ProductDetail(){ProductDetailId = 3, ProductId = 1, Design = "Design 13", Price = 1300, Size = "A3", Quantity = 30},
        
        new ProductDetail(){ProductDetailId = 4, ProductId = 2, Design = "Design 21", Price = 2100, Size = "B1", Quantity = 10},
        new ProductDetail(){ProductDetailId = 5, ProductId = 2, Design = "Design 22", Price = 2200, Size = "B2",Quantity = 20},
        new ProductDetail(){ProductDetailId = 6, ProductId = 2, Design = "Design 23", Price = 2300, Size = "B3", Quantity = 30},
        
        new ProductDetail(){ProductDetailId = 7, ProductId = 3, Design = "Design 31", Price = 3100, Size = "C1", Quantity = 10},
        new ProductDetail(){ProductDetailId = 8, ProductId = 3, Design = "Design 32", Price = 3200, Size = "C2", Quantity = 20},
        new ProductDetail(){ProductDetailId = 9, ProductId = 3, Design = "Design 33", Price = 3300, Size = "C3", Quantity = 30}
    };
}