using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Bookings
{
    public class CancellationRequestConfiguration : BaseEntityConfigurations<CancellationRequest, Guid>
    {
        public override void Configure(EntityTypeBuilder<CancellationRequest> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Reason)
                   .HasMaxLength(1000)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.Property(x => x.RefundAmount)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.BookingId);

            builder.HasOne(x => x.Booking)
                   .WithMany()
                   .HasForeignKey(x => x.BookingId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
