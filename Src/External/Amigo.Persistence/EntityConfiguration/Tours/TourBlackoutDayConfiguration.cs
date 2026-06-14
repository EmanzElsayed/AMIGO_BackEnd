using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{
    public class TourBlackoutDayConfiguration :BaseEntityConfigurations<BlackoutWeekDay,Guid>
    {
        public override void Configure(EntityTypeBuilder<BlackoutWeekDay> builder)
        {
            base.Configure(builder);

          

            builder.HasIndex(t => t.TourId);

            builder.HasOne(t => t.Tour)
              .WithMany(t => t.BlackoutWeekDays)
              .HasForeignKey(t => t.TourId);
        }
    }
}
