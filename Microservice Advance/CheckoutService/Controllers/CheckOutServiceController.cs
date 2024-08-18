using System.Security.Claims;
using CheckoutService.Coordinator;
using CheckoutService.Enums;
using CheckoutService.Models;
using CheckoutService.ServiceImplementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Int32;

namespace CheckoutService.Controllers;

[Route("api/checkout")]
[Authorize(Roles = "User")]
[ApiController]
public class CheckOutServiceController(ICheckOutCoordinator checkOutCoordinator, IMessageService messageService)
    : ControllerBase
{
    private static int _order = 0;

    [HttpPost("process")]
    public async Task<IActionResult> Checkout([FromBody] CheckOutRequestItem checkOutRequest)
    {
        try
        {
            var user = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(user))
                return Unauthorized("Session is timed out. Please login again.");
            if (!TryParse(user, out var userId))
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error has occurred");

            var token = HttpContext.Request.Headers.Authorization;
            if (string.IsNullOrEmpty(token))
                return BadRequest("Auth token is not found in the header");
            
            var result = await checkOutCoordinator.ExecuteCheckOut(userId, token, checkOutRequest.CartItems);
            if (result.Item1)
            {
                SendSuccessNotification(checkOutRequest.CartItems.ToList(), userId);
                return Ok("Order is placed successfully");
            }

            SendFailureNotification($"Error -{result.Item2}");
            return StatusCode(StatusCodes.Status400BadRequest,
                $"Error: {result.Item2}");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }

    private void SendSuccessNotification(List<CartItem> items, int userId)
    {
        _order += 1;
        var orderNotification = new OrderNotification()
        {
            UserId = userId,
            OrderId = _order,
            Status = OrderStatus.Created,
            Products = items
        };
        messageService.PublishSuccessMessage(orderNotification);
    }

    private void SendFailureNotification(string message)
    {
        messageService.PublishFailedMessage(message);
    }
}