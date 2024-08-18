using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace JWTConfiguration;

public static class JwtConfigurationProvider
{
    public static TokenValidationParameters GetTokenValidationParameters()
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
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAud,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    }
}