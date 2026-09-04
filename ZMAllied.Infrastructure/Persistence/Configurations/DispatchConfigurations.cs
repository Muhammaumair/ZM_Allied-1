using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZMAllied.Domain.Entities.Dispatch;

namespace ZMAllied.Infrastructure.Persistence.Configurations
{
    public class DDRConfiguration : IEntityTypeConfiguration<DDR>
    {
        public void Configure(EntityTypeBuilder<DDR> builder)
        {
            builder.ToTable("DDRs");

            builder.HasKey(ddr => ddr.Id);

            builder.Property(ddr => ddr.Lot).HasMaxLength(100);
            builder.Property(ddr => ddr.Item).HasMaxLength(200);
            builder.Property(ddr => ddr.Rent).HasColumnType("decimal(18,2)");
            builder.Property(ddr => ddr.MobileNumber).HasMaxLength(20);
            builder.Property(ddr => ddr.LoadingPoint).HasMaxLength(200);
            builder.Property(ddr => ddr.OffloadingPoint).HasMaxLength(200);
            builder.Property(ddr => ddr.Remarks).HasMaxLength(500);

            builder.Property(ddr => ddr.Weight).HasColumnType("decimal(18,3)");

            builder.HasIndex(ddr => ddr.Date);

            builder.HasOne(ddr => ddr.Company)
                .WithMany(c => c.DDRs)
                .HasForeignKey(ddr => ddr.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ddr => ddr.Bilty)
                .WithMany(b => b.DDRs)
                .HasForeignKey(ddr => ddr.BiltyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ddr => ddr.Vehicle)
                .WithMany(v => v.DDRs)
                .HasForeignKey(ddr => ddr.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ddr => ddr.Broker)
                .WithMany(p => p.BrokerDDRs)
                .HasForeignKey(ddr => ddr.BrokerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ddr => ddr.Party)
                .WithMany(p => p.DispatchDDRs)
                .HasForeignKey(ddr => ddr.PartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
