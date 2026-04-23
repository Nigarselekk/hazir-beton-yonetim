using HazirBeton.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazirBeton.Infrastructure.Persistence.Configurations;

public class ConcreteRequestConfiguration : IEntityTypeConfiguration<ConcreteRequest>
{
    public void Configure(EntityTypeBuilder<ConcreteRequest> builder)
    {
        builder.ToTable("concrete_requests");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.RequesterName).IsRequired().HasMaxLength(200);
        builder.Property(cr => cr.CompanyContactPhone).IsRequired().HasMaxLength(20);
        builder.Property(cr => cr.SiteContactPhone).IsRequired().HasMaxLength(20);
        builder.Property(cr => cr.MaterialType).IsRequired().HasMaxLength(100);
        builder.Property(cr => cr.Note).HasMaxLength(1000);
        builder.Property(cr => cr.CancellationReason).HasMaxLength(500);
        builder.Property(cr => cr.RequestedQuantity).HasPrecision(10, 2);
        builder.Property(cr => cr.UnitPrice).HasPrecision(18, 2);
        builder.Property(cr => cr.TotalAmount).HasPrecision(18, 2);
        builder.Property(cr => cr.WaybillType).IsRequired().HasMaxLength(100);
        builder.Property(cr => cr.DeliveryMethod).IsRequired().HasMaxLength(100);
        builder.Property(cr => cr.DeliveredQuantity).HasPrecision(10, 2);
        builder.Property(cr => cr.Status).HasConversion<string>().IsRequired();

        builder.HasOne(cr => cr.Customer)
            .WithMany()
            .HasForeignKey(cr => cr.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.Site)
            .WithMany(s => s.ConcreteRequests)
            .HasForeignKey(cr => cr.SiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cr => cr.ApprovedBy)
            .WithMany()
            .HasForeignKey(cr => cr.ApprovedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(cr => cr.AssignedBy)
            .WithMany()
            .HasForeignKey(cr => cr.AssignedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(cr => cr.CreatedBy)
            .WithMany()
            .HasForeignKey(cr => cr.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
