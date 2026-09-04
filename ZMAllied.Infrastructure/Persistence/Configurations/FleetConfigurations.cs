using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Infrastructure.Persistence.Configurations
{
    public class VehicleTypeConfiguration : IEntityTypeConfiguration<VehicleType>
    {
        public void Configure(EntityTypeBuilder<VehicleType> builder)
        {
            builder.ToTable("VehicleTypes");

            builder.HasKey(vt => vt.Id);

            builder.Property(vt => vt.Name).IsRequired().HasMaxLength(100);
            builder.Property(vt => vt.Description).HasMaxLength(500);
        }
    }

    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.RegistrationNumber).IsRequired().HasMaxLength(50);
            builder.Property(v => v.Make).HasMaxLength(100);
            builder.Property(v => v.Model).HasMaxLength(100);
            builder.Property(v => v.ChassisNumber).HasMaxLength(100);
            builder.Property(v => v.EngineNumber).HasMaxLength(100);

            builder.Property(v => v.Capacity).HasColumnType("decimal(18,3)");
            builder.Property(v => v.CurrentMileage).HasColumnType("decimal(18,2)");

            builder.HasIndex(v => v.RegistrationNumber).IsUnique();

            builder.HasOne(v => v.VehicleType)
                .WithMany(vt => vt.Vehicles)
                .HasForeignKey(v => v.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Office)
                .WithMany(o => o.Vehicles)
                .HasForeignKey(v => v.OfficeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.ToTable("Drivers");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name).IsRequired().HasMaxLength(150);
            builder.Property(d => d.CNIC).HasMaxLength(50);
            builder.Property(d => d.Phone).HasMaxLength(20);
            builder.Property(d => d.LicenseNumber).HasMaxLength(50);
            builder.Property(d => d.Address).HasMaxLength(500);

            builder.HasIndex(d => d.CNIC);
            builder.HasIndex(d => d.LicenseNumber);
        }
    }

    public class FuelExpenseConfiguration : IEntityTypeConfiguration<FuelExpense>
    {
        public void Configure(EntityTypeBuilder<FuelExpense> builder)
        {
            builder.ToTable("FuelExpenses");

            builder.HasKey(fe => fe.Id);

            builder.Property(fe => fe.FuelType).HasMaxLength(50);
            builder.Property(fe => fe.Remarks).HasMaxLength(500);

            builder.Property(fe => fe.Quantity).HasColumnType("decimal(18,3)");
            builder.Property(fe => fe.Rate).HasColumnType("decimal(18,2)");
            builder.Property(fe => fe.Amount).HasColumnType("decimal(18,2)");
            builder.Property(fe => fe.Odometer).HasColumnType("decimal(18,2)");

            builder.HasOne(fe => fe.Vehicle)
                .WithMany(v => v.FuelExpenses)
                .HasForeignKey(fe => fe.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fe => fe.Trip)
                .WithMany()
                .HasForeignKey(fe => fe.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fe => fe.Supplier)
                .WithMany(s => s.FuelExpenses)
                .HasForeignKey(fe => fe.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class VehicleMaintenanceConfiguration : IEntityTypeConfiguration<VehicleMaintenance>
    {
        public void Configure(EntityTypeBuilder<VehicleMaintenance> builder)
        {
            builder.ToTable("VehicleMaintenances");

            builder.HasKey(vm => vm.Id);

            builder.Property(vm => vm.MaintenanceType).HasMaxLength(100);
            builder.Property(vm => vm.Description).HasMaxLength(500);
            builder.Property(vm => vm.Remarks).HasMaxLength(500);

            builder.Property(vm => vm.Mileage).HasColumnType("decimal(18,2)");
            builder.Property(vm => vm.Cost).HasColumnType("decimal(18,2)");

            builder.HasOne(vm => vm.Vehicle)
                .WithMany(v => v.VehicleMaintenances)
                .HasForeignKey(vm => vm.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(vm => vm.Supplier)
                .WithMany(s => s.VehicleMaintenances)
                .HasForeignKey(vm => vm.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
