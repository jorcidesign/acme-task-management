using AcmeTaskApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
 
namespace AcmeTaskApi.Infrastructure.Persistence.Configurations;
 
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
 
        builder.HasKey(e => e.Id);
 
        builder.Property(e => e.Id)
               .HasColumnName("id")
               .UseIdentityAlwaysColumn();
 
        builder.Property(e => e.Email)
               .HasColumnName("email")
               .HasMaxLength(255)
               .IsRequired();
 
        builder.HasIndex(e => e.Email)
               .IsUnique()
               .HasDatabaseName("uq_users_email");
 
        builder.Property(e => e.FullName)
               .HasColumnName("full_name")
               .HasMaxLength(150)
               .IsRequired();
 
        builder.Property(e => e.PasswordHash)
               .HasColumnName("password_hash")
               .HasMaxLength(72)
               .IsRequired();
 
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
    }
}