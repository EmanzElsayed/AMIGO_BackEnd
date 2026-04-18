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

            builder.Property(x => x.UserId)
              .HasMaxLength(450)
              .IsRequired();

            builder.Property(x => x.Currency)
                   .HasConversion<int>();

            builder.Property(x => x.Status)
                   .HasConversion<int>();

            builder.Property(x => x.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.OrderItems)
                   .WithOne(x => x.Order)
                   .HasForeignKey(x => x.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
