using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZMAllied.Domain.Entities.Trips;
using BiltyEntity = ZMAllied.Domain.Entities.Bilty.Bilty;

namespace ZMAllied.Infrastructure.Persistence.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.ToTable("Trips");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.TripNumber).IsRequired().HasMaxLength(50);
            builder.Property(t => t.LoadingPoint).HasMaxLength(200);
            builder.Property(t => t.OffloadingPoint).HasMaxLength(200);
            builder.Property(t => t.Remarks).HasMaxLength(500);

            builder.Property(t => t.TotalFreight).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Advance).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Balance).HasColumnType("decimal(18,2)");

            builder.HasIndex(t => t.TripNumber).IsUnique();
            builder.HasIndex(t => t.TripDate);

            builder.HasOne(t => t.Company)
                .WithMany(c => c.Trips)
                .HasForeignKey(t => t.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Party)
                .WithMany(p => p.Trips)
                .HasForeignKey(t => t.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Vehicle)
                .WithMany(v => v.Trips)
                .HasForeignKey(t => t.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Driver)
                .WithMany(d => d.Trips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // One Trip can be referenced by many Bilties.
            builder.HasMany(t => t.Bilties)
                .WithOne(b => b.Trip)
                .HasForeignKey(b => b.TripId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class TripExpenseConfiguration : IEntityTypeConfiguration<TripExpense>
    {
        public void Configure(EntityTypeBuilder<TripExpense> builder)
        {
            builder.ToTable("TripExpenses");

            builder.HasKey(te => te.Id);

            builder.Property(te => te.ExpenseType).HasMaxLength(100);
            builder.Property(te => te.Description).HasMaxLength(500);
            builder.Property(te => te.Remarks).HasMaxLength(500);

            builder.Property(te => te.Amount).HasColumnType("decimal(18,2)");

            builder.HasOne(te => te.Trip)
                .WithMany(t => t.TripExpenses)
                .HasForeignKey(te => te.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(te => te.Supplier)
                .WithMany(s => s.TripExpenses)
                .HasForeignKey(te => te.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
