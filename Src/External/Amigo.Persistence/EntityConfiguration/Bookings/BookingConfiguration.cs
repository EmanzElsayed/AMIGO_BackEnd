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


                builder.Property(x => x.UserId)
               .HasMaxLength(450)
               .IsRequired();

                builder.Property(x => x.CustomerName)
                       .HasMaxLength(250)
                       .IsRequired();

                builder.Property(x => x.CustomerEmail)
                       .HasMaxLength(250)
                       .IsRequired();

                builder.Property(x => x.BookingNumber)
                       .HasMaxLength(100);

                builder.Property(x => x.Status)
                       .HasConversion<int>();

                builder.HasIndex(x => x.OrderItemId).IsUnique();
                builder.HasIndex(x => x.UserId);
                builder.HasIndex(x => x.BookingNumber);

                builder.HasOne(x => x.OrderItem)
                       .WithMany()
                       .HasForeignKey(x => x.OrderItemId)
                       .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.User)
                       .WithMany()
                       .HasForeignKey(x => x.UserId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(x => x.Travelers)
                       .WithOne(x => x.Booking)
                       .HasForeignKey(x => x.BookingId)
                       .OnDelete(DeleteBehavior.Cascade);

                  

            }
        }
    
}
