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
        builder.Property(sl => sl.Recipient).IsRequired().HasMaxLength(20);
        builder.Property(sl => sl.Content).IsRequired().HasMaxLength(500);

        builder.HasOne(sl => sl.ConcreteRequest)
            .WithMany(cr => cr.SmsLogs)
            .HasForeignKey(sl => sl.ConcreteRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
