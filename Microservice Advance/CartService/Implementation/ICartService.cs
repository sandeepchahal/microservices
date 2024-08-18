using CartService.Models;

namespace CartService.Implementation;

public interface ICartService
{
    Task<(bool,string?)> AddToCart(int userId,ProductReservationDTO productReservation);
    List<CartReservation> GetCartItems(int userId);
    Dictionary<int, List<CartReservation>>  GetAllItems();
   Task<(bool,string?)> RemoveItem(int userId,int productId, int productDetailId);
   Task<(bool,string?)> AddQuantity(int userId,int productId, int productDetailId, int quantity);
   Task<(bool,string?)> DeleteQuantity(int userId,int productId, int productDetailId, int quantity);

}