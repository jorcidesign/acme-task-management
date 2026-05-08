using AcmeTaskApi.Domain.Enums;
using AcmeTaskApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
 
namespace AcmeTaskApi.Infrastructure.Persistence.Configurations;
 
internal sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");
 
        builder.HasKey(e => e.Id);
 
        builder.Property(e => e.Id)
               .HasColumnName("id")
               .UseIdentityAlwaysColumn();
 
        builder.Property(e => e.UserId)
               .HasColumnName("user_id")
               .IsRequired();
 
        builder.Property(e => e.Title)
               .HasColumnName("title")
               .HasMaxLength(255)
               .IsRequired();
 
        builder.Property(e => e.Description)
               .HasColumnName("description")
               .HasColumnType("text");
 
        builder.Property(e => e.Status)
               .HasColumnName("status")
               .HasDefaultValue(TaskStatus.Pending);
 
        builder.Property(e => e.DueDate)
               .HasColumnName("due_date");
 
        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .HasDefaultValueSql("NOW()")
               .ValueGeneratedOnAdd();
 
        // The DB trigger owns updated_at — EF must never write it on UPDATE.
        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .HasDefaultValueSql("NOW()")
               .ValueGeneratedOnAddOrUpdate()
               .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
 
        // FK → users (matches DDL: ON DELETE RESTRICT, named constraint)
        builder.HasOne(d => d.User)
               .WithMany(p => p.Tasks)
               .HasForeignKey(d => d.UserId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_tasks_user");
 
        // Matches DDL indexes exactly
        builder.HasIndex(e => new { e.UserId, e.Status })
               .HasDatabaseName("idx_tasks_user_status");
 
        builder.HasIndex(e => new { e.UserId, e.DueDate })
               .HasDatabaseName("idx_tasks_user_due")
               .HasFilter("due_date IS NOT NULL");
       // Mapeo de la nueva columna
       builder.Property(e => e.IsDeleted)
               .HasColumnName("is_deleted")
               .HasDefaultValue(false);
       // ¡LA MAGIA DE EF CORE! 
       // Esto filtra automáticamente las tareas borradas en TODOS los queries (GET, UPDATE, etc.)
       builder.HasQueryFilter(e => !e.IsDeleted);
    }
}