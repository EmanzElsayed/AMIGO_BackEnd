using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Baskets
{


    public class BasketConfiguration : BaseEntityConfigurations<Basket, Guid>
    {
        public override void Configure(EntityTypeBuilder<Basket> builder)
        {
            base.Configure(builder);

           

            builder.Property(b => b.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(b => b.UserId)
                   .IsUnique();

            builder.HasMany(b => b.Items)
                   .WithOne(i => i.Basket)
                   .HasForeignKey(i => i.BasketId);
        }
    }
}
