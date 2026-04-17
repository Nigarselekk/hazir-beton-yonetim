using HazirBeton.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazirBeton.Infrastructure.Persistence.Configurations;

public class ConcreteRequestVehicleConfiguration : IEntityTypeConfiguration<ConcreteRequestVehicle>
{
    public void Configure(EntityTypeBuilder<ConcreteRequestVehicle> builder)
    {
        builder.ToTable("concrete_request_vehicles");

        builder.HasKey(crv => crv.Id);

        builder.HasIndex(crv => new { crv.ConcreteRequestId, crv.VehicleId }).IsUnique();

        builder.HasOne(crv => crv.ConcreteRequest)
            .WithMany(cr => cr.ConcreteRequestVehicles)
            .HasForeignKey(crv => crv.ConcreteRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(crv => crv.Vehicle)
            .WithMany(v => v.ConcreteRequestVehicles)
            .HasForeignKey(crv => crv.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
