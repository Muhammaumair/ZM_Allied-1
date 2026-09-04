using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZMAllied.Domain.Entities.Accounting;

namespace ZMAllied.Infrastructure.Persistence.Configurations
{
    public class CashBookEntryConfiguration : IEntityTypeConfiguration<CashBookEntry>
    {
        public void Configure(EntityTypeBuilder<CashBookEntry> builder)
        {
            builder.ToTable("CashBookEntries");

            builder.HasKey(cb => cb.Id);

            builder.Property(cb => cb.Description).HasMaxLength(500);
            builder.Property(cb => cb.ReferenceType).HasMaxLength(100);
            builder.Property(cb => cb.ReferenceNumber).HasMaxLength(100);
            builder.Property(cb => cb.Remarks).HasMaxLength(500);

            builder.Property(cb => cb.Debit).HasColumnType("decimal(18,2)");
            builder.Property(cb => cb.Credit).HasColumnType("decimal(18,2)");
            builder.Property(cb => cb.Balance).HasColumnType("decimal(18,2)");

            builder.HasIndex(cb => cb.Date);
            builder.HasIndex(cb => cb.ReferenceNumber);
            builder.HasIndex(cb => cb.PaymentId)
                .IsUnique()
                .HasFilter("[PaymentId] IS NOT NULL");
            builder.HasIndex(cb => cb.ReceiptId)
                .IsUnique()
                .HasFilter("[ReceiptId] IS NOT NULL");

            builder.HasOne(cb => cb.Payment)
                .WithMany(p => p.CashBookEntries)
                .HasForeignKey(cb => cb.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cb => cb.Receipt)
                .WithMany(r => r.CashBookEntries)
                .HasForeignKey(cb => cb.ReceiptId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PaymentNumber).IsRequired().HasMaxLength(50);
            builder.Property(p => p.PaymentMethod).HasMaxLength(100);
            builder.Property(p => p.ReferenceNumber).HasMaxLength(100);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.Remarks).HasMaxLength(500);

            builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");

            builder.HasIndex(p => p.PaymentNumber).IsUnique();

            builder.HasOne(p => p.Party)
                .WithMany(party => party.Payments)
                .HasForeignKey(p => p.PartyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Supplier)
                .WithMany(s => s.Payments)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
    {
        public void Configure(EntityTypeBuilder<Receipt> builder)
        {
            builder.ToTable("Receipts");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReceiptNumber).IsRequired().HasMaxLength(50);
            builder.Property(r => r.PaymentMethod).HasMaxLength(100);
            builder.Property(r => r.ReferenceNumber).HasMaxLength(100);
            builder.Property(r => r.Description).HasMaxLength(500);
            builder.Property(r => r.Remarks).HasMaxLength(500);

            builder.Property(r => r.Amount).HasColumnType("decimal(18,2)");

            builder.HasIndex(r => r.ReceiptNumber).IsUnique();

            builder.HasOne(r => r.Party)
                .WithMany(p => p.Receipts)
                .HasForeignKey(r => r.PartyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class AccountTransactionConfiguration : IEntityTypeConfiguration<AccountTransaction>
    {
        public void Configure(EntityTypeBuilder<AccountTransaction> builder)
        {
            builder.ToTable("AccountTransactions");

            builder.HasKey(at => at.Id);

            builder.Property(at => at.ReferenceType).HasMaxLength(100);
            builder.Property(at => at.Description).HasMaxLength(500);
            builder.Property(at => at.Remarks).HasMaxLength(500);

            builder.Property(at => at.Debit).HasColumnType("decimal(18,2)");
            builder.Property(at => at.Credit).HasColumnType("decimal(18,2)");
        }
    }
}
