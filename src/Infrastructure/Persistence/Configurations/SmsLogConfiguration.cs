using HazirBeton.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HazirBeton.Infrastructure.Persistence.Configurations;

public class SmsLogConfiguration : IEntityTypeConfiguration<SmsLog>
{
    public void Configure(EntityTypeBuilder<SmsLog> builder)
    {
        builder.ToTable("sms_logs");

        builder.HasKey(sl => sl.Id);

        builder.Property(sl => sl.EventType).HasConversion<string>().IsRequired();
        builder.Property(sl => sl.Status).HasConversion<string>().IsRequired();

        builder.Property(sl => sl.Recipient).IsRequired().HasMaxLength(20);
        builder.Property(sl => sl.Content).IsRequired().HasMaxLength(500);
        builder.Property(sl => sl.LastErrorMessage).HasMaxLength(1000);
        builder.Property(sl => sl.ProviderMessageId).HasMaxLength(100);

        builder.HasOne(sl => sl.ConcreteRequest)
            .WithMany(cr => cr.SmsLogs)
            .HasForeignKey(sl => sl.ConcreteRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        // Idempotency: at most one SMS row per (request, event).
        builder.HasIndex(sl => new { sl.ConcreteRequestId, sl.EventType }).IsUnique();

        // Worker scan: pick up Pending rows whose NextAttemptAt is due.
        builder.HasIndex(sl => new { sl.Status, sl.NextAttemptAt });
    }
}
