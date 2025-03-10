using System.Security.Claims;

namespace BLL.Service.Interfaces;

public interface IJWTTokenService
{
    string GenerateToken(string email, string role);
    ClaimsPrincipal? GetClaimsFromToken(string token);
    string? GetClaimValue(string token, string claimType);

}
