using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Bookings
{


    public class BookingConfiguration : BaseEntityConfigurations<Booking, Guid>
    {
        public override void Configure(EntityTypeBuilder<Booking> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Status)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(x => x.BookingDate)
                   .HasColumnType("timestamp")
                   .IsRequired();

            // ✅ Snapshot fields
            builder.Property(x => x.Date)
                   .IsRequired();

            builder.Property(x => x.Time)
                   .IsRequired();

            // Relationships
            builder.HasOne(x => x.Order)
                   .WithMany()
                   .HasForeignKey(x => x.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AvailableSlots)
                   .WithMany()
                   .HasForeignKey(x => x.AvailableSlotsId)
                   .OnDelete(DeleteBehavior.SetNull); // 🔥 مهم

            // Indexes
            builder.HasIndex(x => x.OrderId);
            builder.HasIndex(x => x.AvailableSlotsId);
            builder.HasIndex(x => x.Status);

            // 🔥 useful query index
            builder.HasIndex(x => new { x.Date, x.Time });
        }
    }
}
