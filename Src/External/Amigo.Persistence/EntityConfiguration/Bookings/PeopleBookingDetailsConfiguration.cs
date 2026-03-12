using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Bookings
{
    
        public class PeopleBookingDetailsConfiguration : BaseEntityConfigurations<PeopleBookingDetails, Guid>
        {
            public override void Configure(EntityTypeBuilder<PeopleBookingDetails> builder)
            {
                base.Configure(builder);

                // Strings
                builder.Property(x => x.FullName)
                       .IsRequired()
                       .HasMaxLength(200);

                builder.Property(x => x.Nationality)
                       .IsRequired()
                       .HasMaxLength(100);

                builder.Property(x => x.PhoneNumber)
                       .HasMaxLength(20);

                // Relationships
                builder.HasOne(x => x.PeopleBooking)
                       .WithMany(z => z.PeopleBookingDetails)
                       .HasForeignKey(x => x.PeopleBookingId)
                       .OnDelete(DeleteBehavior.Cascade);

                // Indexes
                builder.HasIndex(x => x.PeopleBookingId);
            }
        }
    
}
