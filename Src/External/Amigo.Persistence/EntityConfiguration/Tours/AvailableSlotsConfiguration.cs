using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{

    public class AvailableSlotsConfiguration : BaseEntityConfigurations<AvailableSlots, Guid>
    {
        public override void Configure(EntityTypeBuilder<AvailableSlots> builder)
        {
            base.Configure(builder);

           

            

            builder.Property(a => a.StartTime)
                   .HasColumnType("time");

            builder.Property(a => a.EndTime)
                   .HasColumnType("time");

            builder.Property(a => a.AvailableTimeStatus)
                   .HasConversion<int>();

            builder.Property(a => a.MaxCapacity)
                   .IsRequired();

            builder.HasIndex(a => a.TourScheduleId);

            builder.HasIndex(a => new { a.StartTime, a.EndTime });

            builder.HasOne(a => a.TourSchedule)
                .WithMany(t => t.AvailableSlots)
                .HasForeignKey(a => a.TourScheduleId);

        }
    }
}
