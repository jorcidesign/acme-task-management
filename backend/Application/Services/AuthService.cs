using AcmeTaskApi.Application.DTOs.Auth;
using AcmeTaskApi.Application.Interfaces.Repositories;
using AcmeTaskApi.Application.Interfaces.Services;
using AcmeTaskApi.Domain.Exceptions;
using Microsoft.Extensions.Logging;
 
namespace AcmeTaskApi.Application.Services;
 
public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
 
    // Token lifetime in seconds exposed for the response DTO
    private const int TokenExpiresInSeconds = 8 * 3600;
 
    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }
 
    public async Task<AuthResponseDto> AuthenticateAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, ct);
 
        // Constant-time comparison: always verify even if user is null
        // to mitigate user-enumeration timing attacks.
        var isValid = user is not null
                      && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
 
        if (!isValid)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            throw new InvalidCredentialsException();
        }
 
        var token = _tokenService.GenerateToken(user!.Id, user.Email, user.FullName);
 
        _logger.LogInformation("User {UserId} authenticated successfully", user.Id);
 
        return new AuthResponseDto(
            AccessToken: token,
            TokenType: "Bearer",
            ExpiresIn: TokenExpiresInSeconds,
            FullName: user.FullName,
            Email: user.Email
        );
    }
}
 