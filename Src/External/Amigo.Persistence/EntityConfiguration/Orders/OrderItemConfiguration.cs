using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Orders
{

    public class OrderItemConfiguration : BaseEntityConfigurations<OrderItem, Guid>
    {
        public override void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.TourTitle)
                           .HasMaxLength(400)
                           .IsRequired();


            builder.Property(x => x.DestinationName)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(x => x.CurrencyCode)
                   .HasConversion<int>();

            builder.Property(x => x.Language)
                   .HasConversion<int>();

            builder.Property(x => x.MeetingPoint)
                   .HasMaxLength(500);

            builder.Property(x => x.CancelationPolicyType)
                   .HasConversion<int>();

            builder.Property(x => x.RefundPercentage)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.OrderId);

            builder.HasMany(x => x.OrderedPrice)
                   .WithOne(x => x.OrderItem)
                   .HasForeignKey(x => x.OrderItemId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.TravelersDraft)
                       .WithOne(x => x.OrderItem)
                       .HasForeignKey(x => x.OrderItemId)
                       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
