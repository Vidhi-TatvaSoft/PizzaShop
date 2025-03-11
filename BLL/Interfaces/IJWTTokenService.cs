using System.Security.Claims;

namespace BLL.Service.Interfaces;

public interface IJWTTokenService
{
    string GenerateToken(string email, string role);

 string GenerateTokenEmailPassword(string email, string password);
    ClaimsPrincipal? GetClaimsFromToken(string token);
    string? GetClaimValue(string token, string claimType);

}
