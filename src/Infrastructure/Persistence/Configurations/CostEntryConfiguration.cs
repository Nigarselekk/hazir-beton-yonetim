using HazirBeton.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazirBeton.Infrastructure.Persistence.Configurations;

public class CostEntryConfiguration : IEntityTypeConfiguration<CostEntry>
{
    public void Configure(EntityTypeBuilder<CostEntry> builder)
    {
        builder.ToTable("cost_entries");

        builder.HasKey(ce => ce.Id);

        builder.Property(ce => ce.CostItemType).HasConversion<string>().IsRequired();
        builder.Property(ce => ce.Quantity).HasPrecision(10, 2);
        builder.Property(ce => ce.Unit).IsRequired().HasMaxLength(50);
        builder.Property(ce => ce.UnitPrice).HasPrecision(18, 2);
        builder.Property(ce => ce.Total).HasPrecision(18, 2);
        builder.Property(ce => ce.Description).HasMaxLength(1000);
    }
}
