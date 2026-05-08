using System.ComponentModel.DataAnnotations;
using AcmeTaskApi.Domain.Enums;
 
namespace AcmeTaskApi.Application.DTOs.Tasks;
 
public record TaskCreateDto(
    [Required, MaxLength(255)] string Title,
    string? Description,
    DateTime? DueDate
);
 
public record TaskUpdateDto(
    [Required, MaxLength(255)] string Title,
    string? Description,
    [Required] TaskStatus Status,
    DateTime? DueDate
);
 
// Partial update: only patch provided fields
public record TaskPatchStatusDto([Required] TaskStatus Status);
 
public record TaskResponseDto(
    long Id,
    string Title,
    string? Description,
    TaskStatus Status,
    string StatusLabel,       // human-readable: "In Progress"
    DateTime? DueDate,
    bool IsOverdue,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
 
public record TaskFilterDto(
    TaskStatus? Status = null,
    DateTime? DueDateFrom = null,
    DateTime? DueDateTo = null
);