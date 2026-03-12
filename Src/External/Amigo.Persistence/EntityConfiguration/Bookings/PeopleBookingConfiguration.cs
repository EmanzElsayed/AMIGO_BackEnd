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

                // Number of people
                builder.Property(x => x.NoOfPeopleBooking)
                       .IsRequired();

                // Relationships
                builder.HasOne(x => x.Booking)
                       .WithMany(z => z.PeopleBookings)
                       .HasForeignKey(x => x.BookingId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasOne(x => x.Price)
                       .WithMany()
                       .HasForeignKey(x => x.PriceId)
                       .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                builder.HasIndex(x => x.BookingId);
                builder.HasIndex(x => x.PriceId);
            }
        }
    
}
