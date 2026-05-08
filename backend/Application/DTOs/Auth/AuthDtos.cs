using System.ComponentModel.DataAnnotations;
 
namespace AcmeTaskApi.Application.DTOs.Auth;
 
public record LoginRequestDto(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Password
);
 
public record AuthResponseDto(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string FullName,
    string Email
);
 