using AcmeTaskApi.Application.Common;
using AcmeTaskApi.Application.DTOs.Tasks;
using AcmeTaskApi.Application.Interfaces.Repositories;
using AcmeTaskApi.Application.Interfaces.Services;
using AcmeTaskApi.Application.Mappings;
using AcmeTaskApi.Domain.Exceptions;
using AcmeTaskApi.Domain.Models;
using Microsoft.Extensions.Logging;
 
namespace AcmeTaskApi.Application.Services;
 
public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskService> _logger;
 
    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }
 
    public async Task<PagedResult<TaskResponseDto>> GetTasksAsync(
        long userId,
        PaginationParams pagination,
        TaskFilterDto filter,
        CancellationToken ct = default)
    {
        var (items, total) = await _taskRepository.GetPagedByUserAsync(
            userId,
            pagination,
            filter.Status,
            filter.DueDateFrom,
            filter.DueDateTo,
            ct);
 
        return new PagedResult<TaskResponseDto>(
            Items: items.Select(t => t.ToResponseDto()),
            TotalCount: total,
            Page: pagination.Page,
            PageSize: pagination.PageSize
        );
    }
 
    public async Task<TaskResponseDto> GetTaskByIdAsync(long userId, long taskId, CancellationToken ct = default)
    {
        var task = await GetOwnedTaskOrThrowAsync(userId, taskId, ct);
        return task.ToResponseDto();
    }
 
    public async Task<TaskResponseDto> CreateTaskAsync(long userId, TaskCreateDto dto, CancellationToken ct = default)
    {
        var task = new TaskItem
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate?.ToUniversalTime(),
            Status = Domain.Enums.TaskStatus.Pending
        };
 
        var created = await _taskRepository.AddAsync(task, ct);
 
        _logger.LogInformation("Task {TaskId} created for user {UserId}", created.Id, userId);
 
        return created.ToResponseDto();
    }
 
    public async Task<TaskResponseDto> UpdateTaskAsync(
        long userId, long taskId, TaskUpdateDto dto, CancellationToken ct = default)
    {
        var task = await GetOwnedTaskOrThrowAsync(userId, taskId, ct);
 
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.DueDate = dto.DueDate?.ToUniversalTime();
 
        await _taskRepository.UpdateAsync(task, ct);
 
        _logger.LogInformation("Task {TaskId} updated by user {UserId}", taskId, userId);
 
        return task.ToResponseDto();
    }
 
    public async Task PatchStatusAsync(long userId, long taskId, TaskPatchStatusDto dto, CancellationToken ct = default)
    {
        var task = await GetOwnedTaskOrThrowAsync(userId, taskId, ct);
        task.Status = dto.Status;
        await _taskRepository.UpdateAsync(task, ct);
    }
 
   public async Task DeleteTaskAsync(long userId, long taskId, CancellationToken ct = default)
    {
        var task = await GetOwnedTaskOrThrowAsync(userId, taskId, ct);
        
        // SOFT DELETE: En lugar de _taskRepository.DeleteAsync(task, ct);
        task.IsDeleted = true;
        await _taskRepository.UpdateAsync(task, ct);

        _logger.LogInformation("Task {TaskId} soft-deleted by user {UserId}", taskId, userId);
    }
 
    // ------------------------------------------------------------------
    // Private helpers
    // ------------------------------------------------------------------
 
    private async Task<TaskItem> GetOwnedTaskOrThrowAsync(long userId, long taskId, CancellationToken ct)
    {
        var task = await _taskRepository.GetByIdAndUserAsync(taskId, userId, ct);
        if (task is null) throw new NotFoundException(nameof(TaskItem), taskId);
        return task;
    }
}
 