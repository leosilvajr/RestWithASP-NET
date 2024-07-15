using RestWithASPNET.Configurations;
using System.Security.Claims;

namespace RestWithASPNET.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims); 
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
