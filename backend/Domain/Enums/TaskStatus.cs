using NpgsqlTypes;
 
namespace AcmeTaskApi.Domain.Enums;
 
public enum TaskStatus
{
    [PgName("pending")]
    Pending,
 
    [PgName("in_progress")]
    InProgress,
 
    [PgName("completed")]
    Completed
}
 