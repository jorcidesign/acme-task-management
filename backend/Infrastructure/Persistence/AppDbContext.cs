using AcmeTaskApi.Domain.Enums;
using AcmeTaskApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
 
namespace AcmeTaskApi.Infrastructure.Persistence;
 
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
 
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Register the PostgreSQL native ENUM type
        modelBuilder.HasPostgresEnum<TaskStatus>("task_status");
 
        // Apply all IEntityTypeConfiguration<T> classes from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
 
        base.OnModelCreating(modelBuilder);
    }
}
 