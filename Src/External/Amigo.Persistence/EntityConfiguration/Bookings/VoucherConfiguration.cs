using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Bookings
{
    public class VoucherConfiguration : BaseEntityConfigurations<Voucher, Guid>
    {
        public override void Configure(EntityTypeBuilder<Voucher> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.VoucherNumber)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.PdfUrl)
                   .HasMaxLength(1000);

            builder.Property(x => x.QRCode)
                   .HasMaxLength(1000);

            builder.HasIndex(x => x.BookingId).IsUnique();
            builder.HasIndex(x => x.VoucherNumber);

            builder.HasOne(x => x.Booking)
                   .WithOne()
                   .HasForeignKey<Voucher>(x => x.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
