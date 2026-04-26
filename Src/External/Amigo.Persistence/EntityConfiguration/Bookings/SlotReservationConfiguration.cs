using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Bookings
{
    public class SlotReservationConfiguration : BaseEntityConfigurations<SlotReservation, Guid>
    {
        public override void Configure(EntityTypeBuilder<SlotReservation> builder)
        {
            base.Configure(builder);


            builder.Property(x => x.Status)
          .HasConversion<int>();

            builder.HasIndex(x => x.SlotId);

            builder.HasOne(x => x.Slot)
            .WithMany(x => x.SlotReservations)
            .HasForeignKey(x => x.SlotId)
            .OnDelete(DeleteBehavior.Restrict);


        }
}   }
