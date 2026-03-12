using Amigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{

    public class TourConfiguration : BaseEntityConfigurations<Tour, Guid>
    {
        public override void Configure(EntityTypeBuilder<Tour> builder)
        {
            base.Configure(builder);

            


            builder.Property(t => t.Duration)
                   .IsRequired();

            builder.Property(t => t.Discount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(t => t.GuideLanguage)
                   .HasConversion<int>();

            builder.Property(t => t.MeetingPoint)
                   .HasMaxLength(500);

            builder.Property(t => t.IsPitsAllowed)
                    .IsRequired()
                    .HasDefaultValue(false);

            builder.Property(t => t.IsWheelchairAvailable)
                    .IsRequired()
                    .HasDefaultValue(false);

            builder.HasIndex(t => t.DestinationId);

            builder.HasOne(t => t.Destination)
                   .WithMany(d => d.Tours)
                   .HasForeignKey(t => t.DestinationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Cancellation)
                   .WithOne(c => c.Tour)
                   .HasForeignKey<Cancellation>(c => c.TourId);

            builder.HasMany(t => t.AvailableTimes)
                   .WithOne(ts => ts.Tour)
                   .HasForeignKey(ts => ts.TourId);

            builder.HasMany(t => t.Images)
                   .WithOne(i => i.Tour)
                   .HasForeignKey(i => i.TourId);

            builder.HasMany(t => t.Prices)
                   .WithOne(p => p.Tour)
                   .HasForeignKey(p => p.TourId);

            

            builder.HasMany(t => t.Reviews)
                   .WithOne(r => r.Tour)
                   .HasForeignKey(r => r.TourId);
        }
    }
}
