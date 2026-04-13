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

            builder.Property(o => o.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(o => o.PriceType)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(o => o.Quantity)
                   .IsRequired();

            // ✅ Snapshot
            builder.Property(o => o.TourDate)
                   .IsRequired();

            builder.Property(o => o.StartTime)
                   .IsRequired();

            builder.Property(o => o.TourTitle)
                   .HasMaxLength(400)
                   .IsRequired();

            builder.Property(o => o.TourDescription)
                   .HasMaxLength(2000);

            // Relationships
            builder.HasOne(o => o.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(o => o.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.AvailableSlots)
                   .WithMany()
                   .HasForeignKey(o => o.AvailableSlotsId)
                   .OnDelete(DeleteBehavior.SetNull); // 🔥 مهم

            // Indexes
            builder.HasIndex(o => o.OrderId);
            builder.HasIndex(o => o.AvailableSlotsId);
        }
    }
}
