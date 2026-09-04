using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZMAllied.Domain.Entities.Organization;

namespace ZMAllied.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
            builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
            builder.Property(c => c.ContactPerson).HasMaxLength(100);
            builder.Property(c => c.Phone).HasMaxLength(20);
            builder.Property(c => c.Address).HasMaxLength(500);
            builder.Property(c => c.Rate).HasColumnType("decimal(18,2)");

            builder.HasIndex(c => c.Code).IsUnique();
        }
    }

    public class OfficeConfiguration : IEntityTypeConfiguration<Office>
    {
        public void Configure(EntityTypeBuilder<Office> builder)
        {
            builder.ToTable("Offices");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name).IsRequired().HasMaxLength(150);
            builder.Property(o => o.Code).IsRequired().HasMaxLength(50);
            builder.Property(o => o.Address).HasMaxLength(500);
            builder.Property(o => o.City).HasMaxLength(100);
            builder.Property(o => o.Phone).HasMaxLength(20);
            builder.Property(o => o.Email).HasMaxLength(150);

            builder.HasIndex(o => o.Code).IsUnique();

            builder.HasOne(o => o.Company)
                .WithMany(c => c.Offices)
                .HasForeignKey(o => o.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class CompanyRateConfiguration : IEntityTypeConfiguration<CompanyRate>
    {
        public void Configure(EntityTypeBuilder<CompanyRate> builder)
        {
            builder.ToTable("CompanyRates");

            builder.HasKey(cr => cr.Id);

            builder.Property(cr => cr.RatePerTon).HasColumnType("decimal(18,2)");

            builder.HasIndex(cr => cr.CompanyId).IsUnique();

            builder.HasOne(cr => cr.Company)
                .WithOne(c => c.CompanyRate)
                .HasForeignKey<CompanyRate>(cr => cr.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
