using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserService.Models;

namespace UserService.Implementation;

public class UserManageService:IUserManageService
{
    private readonly List<User> _users = PredefinedUsers.Users;

    public User? GetUser(int userId)
    {
        return _users.FirstOrDefault(u => u.UserId == userId);
    }

    public string? Authenticate(string username, string password)
    {
        var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user == null)
            return null;

        var token = GenerateJwtToken(user);
        return token;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        var jwtAud = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT_KEY environment variable is not set.");
        }

        if (string.IsNullOrEmpty(jwtAud))
        {
            throw new InvalidOperationException("JWT_AUDIENCE environment variable is not set.");
 
        }
        if (string.IsNullOrEmpty(jwtIssuer))
        {
            throw new InvalidOperationException("JWT_ISSUER environment variable is not set.");
 
        }
        
        if (jwtKey is null)
            throw new ConfigurationErrorsException("JWT key not found ");
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAud,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}