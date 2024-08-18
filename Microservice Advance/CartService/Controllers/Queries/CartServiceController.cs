using System.Security.Claims;
using CartService.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Int32;

namespace CartService.Controllers.Queries;

[Route("api/cart")]
[Authorize(Roles = "User")]
[ApiController]
public class CartServiceController(ICartService cartService):ControllerBase
{
    [HttpGet("get-all")]
    public IActionResult GetCartItems()
    {
        try
        {
            var user = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(user))
                return Unauthorized("Session is timed out. Please login again.");
            if (!TryParse(user, out var userId))
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error has occurred");
            var items = cartService.GetCartItems(userId);
            if (items.Count != 0)
                return Ok(items);
            return NotFound("No items are available in the cart");

        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "An error has occurred");
        }
    }

}