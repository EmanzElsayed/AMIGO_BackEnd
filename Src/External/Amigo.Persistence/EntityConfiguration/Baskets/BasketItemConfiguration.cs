using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Baskets
{
   

    public class BasketItemConfiguration : BaseEntityConfigurations<BasketItem, Guid>
    {
        public override void Configure(EntityTypeBuilder<BasketItem> builder)
        {
            base.Configure(builder);


           


            builder.Property(b => b.Price)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(b => b.BasketId);

            builder.HasIndex(b => b.AvailableSlotsId);

            builder.HasOne(b => b.AvailableSlots)
                   .WithMany()
                   .HasForeignKey(b => b.AvailableSlotsId);
        }
    }
}
