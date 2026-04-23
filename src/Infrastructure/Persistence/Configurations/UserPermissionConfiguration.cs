using HazirBeton.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazirBeton.Infrastructure.Persistence.Configurations;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("user_permissions");

        builder.HasKey(up => new { up.UserId, up.Permission });

        builder.Property(up => up.Permission).HasConversion<string>().IsRequired();

        builder.HasOne(up => up.User)
            .WithMany(u => u.Permissions)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
