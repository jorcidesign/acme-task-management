using AcmeTaskApi.Domain.Models;
using AcmeTaskApi.Application.Common;
using AcmeTaskApi.Domain.Enums;
 
namespace AcmeTaskApi.Application.Interfaces.Repositories;
 
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<User> AddAsync(User user, CancellationToken ct = default);
}
 
public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAndUserAsync(long taskId, long userId, CancellationToken ct = default);
 
    Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetPagedByUserAsync(
        long userId,
        PaginationParams pagination,
        TaskStatus? statusFilter = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        CancellationToken ct = default);
 
    Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct = default);
    Task UpdateAsync(TaskItem task, CancellationToken ct = default);
    Task DeleteAsync(TaskItem task, CancellationToken ct = default);
}
 