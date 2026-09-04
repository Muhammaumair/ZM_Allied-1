using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZMAllied.Domain.Entities.Parties;

namespace ZMAllied.Infrastructure.Persistence.Configurations
{
    public class PartyConfiguration : IEntityTypeConfiguration<Party>
    {
        public void Configure(EntityTypeBuilder<Party> builder)
        {
            builder.ToTable("Parties");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.ContactPerson).HasMaxLength(100);
            builder.Property(p => p.Phone).HasMaxLength(20);
            builder.Property(p => p.Email).HasMaxLength(150);
            builder.Property(p => p.Address).HasMaxLength(500);
            builder.Property(p => p.NTN).HasMaxLength(50);
            builder.Property(p => p.CNIC).HasMaxLength(50);

            builder.HasIndex(p => p.Name);
            builder.HasIndex(p => p.Phone);
        }
    }

    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(150);
            builder.Property(s => s.ContactPerson).HasMaxLength(100);
            builder.Property(s => s.Phone).HasMaxLength(20);
            builder.Property(s => s.Email).HasMaxLength(150);
            builder.Property(s => s.Address).HasMaxLength(500);
            builder.Property(s => s.NTN).HasMaxLength(50);
            builder.Property(s => s.PaymentTerms).HasMaxLength(200);

            builder.HasIndex(s => s.Name);
        }
    }

    public class PartyTransactionConfiguration : IEntityTypeConfiguration<PartyTransaction>
    {
        public void Configure(EntityTypeBuilder<PartyTransaction> builder)
        {
            builder.ToTable("PartyTransactions");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.ReferenceType).HasMaxLength(100);
            builder.Property(pt => pt.Description).HasMaxLength(500);
            builder.Property(pt => pt.Remarks).HasMaxLength(500);

            builder.Property(pt => pt.Debit).HasColumnType("decimal(18,2)");
            builder.Property(pt => pt.Credit).HasColumnType("decimal(18,2)");

            builder.HasOne(pt => pt.Party)
                .WithMany(p => p.PartyTransactions)
                .HasForeignKey(pt => pt.PartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
