using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZMAllied.Domain.Entities.Bilty;

namespace ZMAllied.Infrastructure.Persistence.Configurations
{
    public class BiltyConfiguration : IEntityTypeConfiguration<Bilty>
    {
        public void Configure(EntityTypeBuilder<Bilty> builder)
        {
            builder.ToTable("Bilties");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BiltyNumber).IsRequired().HasMaxLength(50);
            builder.Property(b => b.LoadingPoint).HasMaxLength(200);
            builder.Property(b => b.OffloadingPoint).HasMaxLength(200);
            builder.Property(b => b.PO_Number).HasMaxLength(100);
            builder.Property(b => b.PaymentTerms).HasMaxLength(200);
            builder.Property(b => b.Remarks).HasMaxLength(500);

            builder.Property(b => b.Freight).HasColumnType("decimal(18,2)");
            builder.Property(b => b.Advance).HasColumnType("decimal(18,2)");
            builder.Property(b => b.Balance).HasColumnType("decimal(18,2)");

            builder.HasIndex(b => b.BiltyNumber).IsUnique();
            builder.HasIndex(b => b.BiltyDate);
            builder.HasIndex(b => b.TripId)
                .IsUnique()
                .HasFilter("[TripId] IS NOT NULL");

            builder.HasOne(b => b.Office)
                .WithMany(o => o.Bilties)
                .HasForeignKey(b => b.OfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Consignor)
                .WithMany(p => p.ConsignorBilties)
                .HasForeignKey(b => b.ConsignorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Consignee)
                .WithMany(p => p.ConsigneeBilties)
                .HasForeignKey(b => b.ConsigneeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Broker)
                .WithMany(p => p.BrokerBilties)
                .HasForeignKey(b => b.BrokerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Vehicle)
                .WithMany(v => v.Bilties)
                .HasForeignKey(b => b.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Driver)
                .WithMany(d => d.Bilties)
                .HasForeignKey(b => b.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class BiltyItemConfiguration : IEntityTypeConfiguration<BiltyItem>
    {
        public void Configure(EntityTypeBuilder<BiltyItem> builder)
        {
            builder.ToTable("BiltyItems");

            builder.HasKey(bi => bi.Id);

            builder.Property(bi => bi.Description).HasMaxLength(500);
            builder.Property(bi => bi.Unit).HasMaxLength(50);

            builder.Property(bi => bi.Quantity).HasColumnType("decimal(18,3)");
            builder.Property(bi => bi.Weight).HasColumnType("decimal(18,3)");
            builder.Property(bi => bi.Rate).HasColumnType("decimal(18,2)");
            builder.Property(bi => bi.Amount).HasColumnType("decimal(18,2)");

            builder.HasOne(bi => bi.Bilty)
                .WithMany(b => b.BiltyItems)
                .HasForeignKey(bi => bi.BiltyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
