namespace CartService.Models;


public class ProductDetailContext
{
    public int ProductDetailId { get; set; }
    public int Quantity { get; set; }
}

public class ProductReservationDTO:ProductDetailContext
{
    public int ProductId { get; set; }
}

public class CartReservation : ProductReservationDTO
{
    public DateTime ExpirationTime { get;} = DateTime.Now.AddMinutes(5);
}
