using CartService.Models;

namespace CartService.Implementation;

public class CartServiceAction(IHttpClientFactory httpClientFactory):ICartService
{
    private static Dictionary<int, List<CartReservation>> _productReservation = new();
    private readonly HttpClient _client = httpClientFactory.CreateClient("ProductDetailServiceClient");
    
    public async Task<(bool,string?)> AddToCart(int userId, ProductReservationDTO productReservation)
    {
        try
        {
            var result = await IsQuantityAvailable(productReservation.ProductDetailId,
                productReservation.Quantity);
            
            if (!result.Item1)
                return result;

            var cart = new CartReservation()
            {
                ProductId = productReservation.ProductId,
                Quantity = productReservation.Quantity,
                ProductDetailId = productReservation.ProductDetailId
            };
            if (!_productReservation.ContainsKey(userId))
            {
                _productReservation[userId] = [cart];

            }
            else
            {
                _productReservation[userId].Add(cart);
            }

            return (true, "Item(s) is added to the cart");
        }
        catch (Exception)
        {
            return (false, "An error has occurred while processing the request");
        }
    }

    public List<CartReservation> GetCartItems(int userId)
    {
        try
        {
            var items = _productReservation.GetValueOrDefault(userId);
            return items ?? [];
        }
        catch (Exception)
        {
            return [];
        }
    }

    public Dictionary<int, List<CartReservation>>  GetAllItems()
    {
        try
        {
            return _productReservation;
        }
        catch (Exception)
        {
            return [];
        }
    }
    public async Task<(bool,string?)> RemoveItem(int userId,int productId, int productDetailId)
    {
        try
        {
            if (!_productReservation.TryGetValue(userId, out List<CartReservation>? value)) 
                return (false,$"No items in the cart for user id - {userId}");
            var result = value
                .Find(col => col.ProductId == productId 
                             && col.ProductDetailId == productDetailId);
            if (result == null) return (false,$"No items in the cart are available for product Id - {productId} ");
            var productDetailResponse = await TryUpdateProductQuantity(result.ProductDetailId, result.Quantity);
            if (!productDetailResponse.Item1) return (false, productDetailResponse.Item2);
            value.Remove(result);
            return (true, $"item is removed from cart successfully");

        }
        catch (Exception)
        {
            return (false,$"An error has occurred while processing the request");
        }
    }

    public async Task<(bool,string?)> AddQuantity(int userId,int productId, int productDetailId, int quantity)
    {
        try
        {
            if (!_productReservation.TryGetValue(userId, out List<CartReservation>? value)) 
                return (false,$"No items in the cart for user id - {userId}");
            var result = value
                .Find(col => col.ProductId == productId 
                             && col.ProductDetailId == productDetailId);
            if (result == null) return (false,$"No items in the cart are available for product Id - {productId} ");
            var productResponse = await TryUpdateProductQuantity(result.ProductId, -quantity);
            if (!productResponse.Item1) return (false, productResponse.Item2);
            result.Quantity += quantity;
            return (true, $"item is removed from cart successfully");
        }
        catch (Exception)
        {
            return (false,$"An error has occurred while processing the request");
        }
    }

    public async Task<(bool,string?)> DeleteQuantity(int userId,int productId, int productDetailId, int quantity)
    {
        try
        {
            if (!_productReservation.TryGetValue(userId, out List<CartReservation>? value)) 
                return (false,$"No items in the cart for user id - {userId}");
            var result = value
                .Find(col => col.ProductId == productId 
                             && col.ProductDetailId == productDetailId);
            if (result == null) return (false,$"No items in the cart are available for product Id - {productId} ");
            var productResponse = await TryUpdateProductQuantity(result.ProductId, quantity);
            if (!productResponse.Item1) return (false, productResponse.Item2);
            result.Quantity -= quantity;
            return (true,$"item is updated successfully");
        }
        catch (Exception)
        {
            return (false,$"An error has occurred while processing the request");
        }
    }

    private async Task<(bool,string?)> IsQuantityAvailable(int productDetailId, int quantity)
    {
        try
        {
            ProductDetailContext productDetailContext =
                new() { ProductDetailId = productDetailId, Quantity = quantity };
            var response = await _client.PostAsJsonAsync($"check-quantity",productDetailContext);
            var responseResult = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, responseResult);
        }
        catch (Exception)
        {
            return (false, "An error has occurred while processing the request");
        }
    }
    
    private async Task<(bool,string?)> TryUpdateProductQuantity(int productDetailId, int quantity)
    {
        try
        {
            ProductDetailContext productDetailContext =
                new() { ProductDetailId = productDetailId, Quantity = quantity };
            var response = await _client.PutAsJsonAsync($"update-quantity", productDetailContext);
            var responseResult = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, responseResult);
        }
        catch (Exception)
        {
            return (false, "An error has occurred while processing the request");
        }
    }
}