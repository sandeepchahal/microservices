using Microsoft.AspNetCore.Mvc;
using UserService.Implementation;
using UserService.Models;

namespace UserService.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthServiceController(IUserManageService userManageService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var token = userManageService.Authenticate(model.Username, model.Password);
        if (token == null)
            return Unauthorized("User Name or Password is incorrect!");

        return Ok(new { Token = token });
    }
}


