using AcmeTaskApi.Application.DTOs.Tasks;
using AcmeTaskApi.Domain.Enums;
using AcmeTaskApi.Domain.Models;
 
namespace AcmeTaskApi.Application.Mappings;
 
/// <summary>
/// Centralized, static mapping logic. Avoids AutoMapper overhead for a simple MVP
/// while keeping mapping in one place (Single Responsibility).
/// </summary>
public static class TaskMapper
{
    public static TaskResponseDto ToResponseDto(this TaskItem task) => new(
        Id: task.Id,
        Title: task.Title,
        Description: task.Description,
        Status: task.Status,
        StatusLabel: task.Status.ToLabel(),
        DueDate: task.DueDate,
        IsOverdue: task.DueDate.HasValue
                   && task.DueDate.Value < DateTime.UtcNow
                   && task.Status != TaskStatus.Completed,
        CreatedAt: task.CreatedAt,
        UpdatedAt: task.UpdatedAt
    );
 
    private static string ToLabel(this TaskStatus status) => status switch
    {
        TaskStatus.Pending    => "Pending",
        TaskStatus.InProgress => "In Progress",
        TaskStatus.Completed  => "Completed",
        _                     => status.ToString()
    };
}
 