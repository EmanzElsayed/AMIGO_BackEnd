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
                       .HasColumnType("timestamp");

                // Relationships
                builder.HasOne(x => x.Order)
                       .WithMany()
                       .HasForeignKey(x => x.OrderId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(x => x.AvailableSlots)
                       .WithMany()
                       .HasForeignKey(x => x.AvailableSlotsId)
                       .OnDelete(DeleteBehavior.Restrict);

                // Indexes for fast queries
                builder.HasIndex(x => x.OrderId);
                builder.HasIndex(x => x.AvailableSlotsId);
                builder.HasIndex(x => x.Status);

                
            }
        }
    
}
