using HazirBeton.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazirBeton.Infrastructure.Persistence.Configurations;

public class VehiclePersonnelConfiguration : IEntityTypeConfiguration<VehiclePersonnel>
{
    public void Configure(EntityTypeBuilder<VehiclePersonnel> builder)
    {
        builder.ToTable("vehicle_personnel");

        builder.HasKey(vp => vp.Id);

        builder.HasIndex(vp => new { vp.VehicleId, vp.PersonnelId }).IsUnique();

        builder.Property(vp => vp.AssignmentType).HasConversion<string>().IsRequired();

        builder.HasOne(vp => vp.Vehicle)
            .WithMany(v => v.VehiclePersonnel)
            .HasForeignKey(vp => vp.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vp => vp.Personnel)
            .WithMany(p => p.VehiclePersonnel)
            .HasForeignKey(vp => vp.PersonnelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
