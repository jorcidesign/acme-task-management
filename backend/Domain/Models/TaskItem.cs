using AcmeTaskApi.Domain.Enums;
 
namespace AcmeTaskApi.Domain.Models;
 
// Named TaskItem to avoid collision with System.Threading.Tasks.Task
public class TaskItem : BaseEntity
{
    public long UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public DateTime? DueDate { get; set; }
 // Nueva propiedad para el Soft Delete
    public bool IsDeleted { get; set; } = false;
    // Navigation property
    public User User { get; set; } = null!;
}
 