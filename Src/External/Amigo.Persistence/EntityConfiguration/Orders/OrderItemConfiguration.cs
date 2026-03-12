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
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(o => o.OrderId);
            builder.HasIndex(o => o.AvailableSlotsId);

            builder.HasOne(o => o.AvailableSlots)
                   .WithMany()
                   .HasForeignKey(o => o.AvailableSlotsId);
        }
    }
}
