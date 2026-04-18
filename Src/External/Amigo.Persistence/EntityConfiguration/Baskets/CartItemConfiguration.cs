using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Baskets
{
   

    public class CartItemConfiguration : BaseEntityConfigurations<CartItem, Guid>
    {
        public override void Configure(EntityTypeBuilder<CartItem> builder)
        {
            base.Configure(builder);



            builder.Property(x => x.Language)
               .HasConversion<int>();

            builder.Property(x => x.TourTitle)
                   .HasMaxLength(400)
                   .IsRequired();

            builder.Property(x => x.DestinationName)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(x => x.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.CartId);
            builder.HasIndex(x => x.TourId);
            builder.HasIndex(x => x.SlotId);

            builder.HasOne(x => x.Cart)
                   .WithMany(x => x.Items)
                   .HasForeignKey(x => x.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Tour)
                   .WithMany()
                   .HasForeignKey(x => x.TourId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Slot)
                   .WithMany()
                   .HasForeignKey(x => x.SlotId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Prices)
                   .WithOne(x => x.CartItem)
                   .HasForeignKey(x => x.CartItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
