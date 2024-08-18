using CheckoutService.Enums;

namespace CheckoutService.Models;

public class CartItem
{
    public int ProductId { get; set; }
    public int ProductDetailId { get; set; }
    public int Quantity { get; set; }
}
public class CheckOutRequestItem
{
    public List<CartItem> CartItems { get; set; }
}



public class OrderNotification
{
    public int UserId { get; set; }
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public List<CartItem> Products { get; set; }
}
