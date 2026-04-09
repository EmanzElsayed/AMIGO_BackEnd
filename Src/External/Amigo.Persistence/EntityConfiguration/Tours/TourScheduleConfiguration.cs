using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{
    

    public class TourScheduleConfiguration : BaseEntityConfigurations<TourSchedule, Guid>
    {
        public override void Configure(EntityTypeBuilder<TourSchedule> builder)
        {
            base.Configure(builder);



            builder.Property(t => t.StartDate)
                   .HasColumnType("date");

            builder.Property(t => t.EndDate)
                   .HasColumnType("date");

            builder.Property(t => t.AvailableDateStatus)
                   .HasConversion<int>();


            builder.HasIndex(t => t.TourId);
            builder.HasIndex(t => new { t.StartDate, t.EndDate });


            builder.HasOne(t => t.Tour)
                .WithMany(t => t.AvailableTimes)
                .HasForeignKey(t => t.TourId);


            builder.HasMany(s => s.AvailableSlots)
                   .WithOne(a => a.TourSchedule)
                   .HasForeignKey(a => a.TourScheduleId);
        }
    }
}

