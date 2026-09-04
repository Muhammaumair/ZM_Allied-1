using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZMAllied.Domain.Entities.Audit;

namespace ZMAllied.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.Action).IsRequired().HasMaxLength(100);
            builder.Property(al => al.EntityName).IsRequired().HasMaxLength(150);
            builder.Property(al => al.EntityId).HasMaxLength(100);
            builder.Property(al => al.IpAddress).HasMaxLength(50);

            builder.Property(al => al.OldValues).HasColumnType("nvarchar(max)");
            builder.Property(al => al.NewValues).HasColumnType("nvarchar(max)");

            builder.HasOne(al => al.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
