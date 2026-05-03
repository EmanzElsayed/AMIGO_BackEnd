using Amigo.Persistence.EntityConfiguration;

public class VoucherConfiguration : BaseEntityConfigurations<Voucher, Guid>
{
    public override void Configure(EntityTypeBuilder<Voucher> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.VoucherNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Token)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.QRCodeBase64)
            .IsRequired();

        builder.Property(x => x.SentAt)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.IsSentByEmail)
            .HasDefaultValue(false);

        builder.HasIndex(x => x.BookingId)
            .IsUnique();

        builder.HasIndex(x => x.VoucherNumber);

        builder.HasIndex(x => x.Token)
            .IsUnique();

        builder.HasOne(x => x.Booking)
            .WithOne(x => x.Voucher)
            .HasForeignKey<Voucher>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}