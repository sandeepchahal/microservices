using System.Net.Http.Headers;
using CheckoutService.Models;

namespace CheckoutService.Coordinator;

public class CheckOutCoordinator(IHttpClientFactory httpClientFactory)
    : ICheckOutCoordinator
{
    private readonly HttpClient _productDetailClient = httpClientFactory.CreateClient("ProductDetailServiceClient");
    private readonly HttpClient _cartClient = httpClientFactory.CreateClient("CartServiceClient");
    private string authToken;

    public async Task<(bool, string)> ExecuteCheckOut(int userId, string authorizationToken,
        IEnumerable<CartItem> cartItems)
    {
        try
        {
            authToken = authorizationToken;
            foreach (var item in cartItems)
            {
                var productReserve = await ReserveProduct(item);
                if (productReserve.Item1) 
                    continue;
                await CompensateQuantity(item);
                return productReserve;
            }

            return (true,"");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #region Product Service

    private async Task<(bool, string)> ReserveProduct(CartItem item)
    {
        var productInfo = new ProductQuantityContext()
            { ProductDetailId = item.ProductDetailId, Quantity = -item.Quantity };
        var response = await _productDetailClient.PostAsJsonAsync("reserve", productInfo);
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, message);
        }

        var paymentProcess = await ProcessPayment(item);
        if (paymentProcess.Item1) return (true,"");
        await CompensateQuantity(item);
        return paymentProcess;
    }

    private async Task<(bool, string)> CompensateQuantity(CartItem item)
    {
        var productInfo = new ProductQuantityContext()
        {
            ProductDetailId = item.ProductDetailId,
            Quantity = item.Quantity
        };
        var response = await _productDetailClient.PutAsJsonAsync("update-quantity", productInfo);
        var message = await response.Content.ReadAsStringAsync();
        return (response.IsSuccessStatusCode, message);
    }

    #endregion

    #region Payment

    private async Task<(bool, string)> ProcessPayment(CartItem item)
    {
        await Task.Delay(10000);
        var cartResponse = await RemoveFromCart(item);
        if (cartResponse.Item1) 
            return (true,"");
        await CompensatePayment(item);
        await CompensateQuantity(item);
        return cartResponse;
    }

    private async Task<(bool, string)> CompensatePayment(CartItem item)
    {
        await Task.Delay(10000);

        return (true,"");
    }

    #endregion

    #region Cart

    private async Task<(bool, string)> RemoveFromCart(CartItem item)
    {
        try
        {
            _cartClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken[7..]);
            var removeCartUrl = $"remove/{item.ProductId}/{item.ProductDetailId}";
            var response = await _cartClient.DeleteAsync(removeCartUrl);
            var message = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, message);
        }
        catch (Exception)
        {
            return (false, "An error has occured while compensating the cart");
        }
    }

    private async Task<(bool, string)> CompensateCart(CartItem cartItem)
    {
        try
        {
            var cartInfo = new CartItem()
            {
                ProductDetailId = cartItem.ProductDetailId, ProductId = cartItem.ProductId, Quantity = cartItem.Quantity
            };
            var response = await _cartClient.PostAsJsonAsync("add", cartInfo);
            var message = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, message);
        }
        catch (Exception)
        {
            return (false, "An error has occured while compensating the cart");
        }
    }

    #endregion
}