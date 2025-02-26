using System.Security.Claims;

namespace BLL.Service.Interfaces;

public interface JWTTokenInterface
{
    string GenerateToken(string email, string role);
    ClaimsPrincipal? GetClaimsFromToken(string token);
   string? GetClaimValue(string token, string claimType);
}
