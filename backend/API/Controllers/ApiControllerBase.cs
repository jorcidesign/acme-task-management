using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AcmeTaskApi.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected long CurrentUserId
    {
        get
        {
            // Buscamos el claim de Microsoft primero, y si no, el estándar JWT 'sub'
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                      ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? throw new InvalidOperationException("JWT 'sub' claim is missing.");
            
            return long.Parse(sub);
        }
    }
}