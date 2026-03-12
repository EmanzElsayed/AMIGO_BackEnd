using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Orders
{

    public class OrderConfiguration : BaseEntityConfigurations<Order, Guid>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

           

            builder.Property(o => o.Currency)
                   .HasConversion<int>();

            builder.Property(o => o.Status)
                   .HasConversion<int>();

            builder.Property(o => o.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(o => o.UserId);

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId);
        }
    }
}
