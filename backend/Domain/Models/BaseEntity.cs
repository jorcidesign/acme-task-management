namespace AcmeTaskApi.Domain.Models;
 
/// <summary>
/// Base class for all entities. Centralizes audit fields to avoid repetition.
/// </summary>
public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
 