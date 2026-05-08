using AcmeTaskApi.Application.DTOs.Auth;
using AcmeTaskApi.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcmeTaskApi.API.Controllers;

[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        var response = await _authService.AuthenticateAsync(request, ct);
        return Ok(response);
    }
}