using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Bookings
{
    public class TravelerConfiguration : BaseEntityConfigurations<Traveler, Guid>
    {
        public override void Configure(EntityTypeBuilder<Traveler> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.FullName)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(x => x.Nationality)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.PassportNumber)
                   .HasMaxLength(100);

            builder.Property(x => x.PhoneNumber)
                   .HasMaxLength(50);

            builder.HasIndex(x => x.BookingId);
        }
    }
}
