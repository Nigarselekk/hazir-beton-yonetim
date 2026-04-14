using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazirBeton.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Plate).IsRequired().HasMaxLength(20);
        builder.HasIndex(v => v.Plate).IsUnique();

        builder.Property(v => v.Type).HasConversion<string>().IsRequired();
        builder.Property(v => v.Status).HasConversion<string>().IsRequired();
    }
}
