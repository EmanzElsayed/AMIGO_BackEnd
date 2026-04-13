using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Bookings
{

    public class PeopleBookingConfiguration : BaseEntityConfigurations<PeopleBooking, Guid>
    {
        public override void Configure(EntityTypeBuilder<PeopleBooking> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.NoOfPeopleBooking)
                   .IsRequired();

            // ✅ Snapshot
            builder.Property(x => x.PriceAtBooking)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(x => x.PriceType)
                   .HasMaxLength(100)
                   .IsRequired();

            // Relationships
            builder.HasOne(x => x.Booking)
                   .WithMany(z => z.PeopleBookings)
                   .HasForeignKey(x => x.BookingId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(x => x.BookingId);
        }
    }

}
