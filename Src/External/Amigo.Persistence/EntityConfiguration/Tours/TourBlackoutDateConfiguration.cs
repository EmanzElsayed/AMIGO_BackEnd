using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{
    public class TourBlackoutDateConfiguration :BaseEntityConfigurations<BlackoutDate,Guid>
    {
        public override void Configure(EntityTypeBuilder<BlackoutDate> builder)
        {
            base.Configure(builder);

            builder.Property(t => t.Date)
                   .HasColumnType("date");

            builder.HasIndex(t => t.TourId);

            builder.HasOne(t => t.Tour)
              .WithMany(t => t.BlackoutDates)
              .HasForeignKey(t => t.TourId);

        }
    }
}
