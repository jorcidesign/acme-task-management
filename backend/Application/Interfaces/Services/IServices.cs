using AcmeTaskApi.Application.Common;
using AcmeTaskApi.Application.DTOs.Auth;
using AcmeTaskApi.Application.DTOs.Tasks;
using AcmeTaskApi.Domain.Enums;
 
namespace AcmeTaskApi.Application.Interfaces.Services;
 
public interface IAuthService
{
    /// <summary>Validates credentials and returns a signed JWT. Throws InvalidCredentialsException on failure.</summary>
    Task<AuthResponseDto> AuthenticateAsync(LoginRequestDto request, CancellationToken ct = default);
}
 
public interface ITaskService
{
    Task<PagedResult<TaskResponseDto>> GetTasksAsync(
        long userId,
        PaginationParams pagination,
        TaskFilterDto filter,
        CancellationToken ct = default);
 
    Task<TaskResponseDto> GetTaskByIdAsync(long userId, long taskId, CancellationToken ct = default);
    Task<TaskResponseDto> CreateTaskAsync(long userId, TaskCreateDto dto, CancellationToken ct = default);
    Task<TaskResponseDto> UpdateTaskAsync(long userId, long taskId, TaskUpdateDto dto, CancellationToken ct = default);
    Task PatchStatusAsync(long userId, long taskId, TaskPatchStatusDto dto, CancellationToken ct = default);
    Task DeleteTaskAsync(long userId, long taskId, CancellationToken ct = default);
}
 
public interface ITokenService
{
    string GenerateToken(long userId, string email, string fullName);
}
 