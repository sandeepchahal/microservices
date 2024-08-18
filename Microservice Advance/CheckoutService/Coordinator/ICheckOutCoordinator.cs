using CheckoutService.Models;

namespace CheckoutService.Coordinator;

public interface ICheckOutCoordinator
{
    Task<(bool,string)> ExecuteCheckOut(int userId,string authorizationToken,IEnumerable<CartItem> cartItems);
}