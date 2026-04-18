using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Baskets
{


    public class CartConfiguration : BaseEntityConfigurations<Cart, Guid>
    {
        public override void Configure(EntityTypeBuilder<Cart> builder)
        {
            base.Configure(builder);



            builder.Property(x => x.UserId)
              .HasMaxLength(450);

            builder.Property(x => x.CartToken)
                   .HasMaxLength(200);

            builder.Property(x => x.CurrencyCode)
                   .HasConversion<int>();

            builder.Property(x => x.TotalAmount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(x => x.LastUpdatedAt)
                   .IsRequired();

            builder.Property(x => x.ExpiresAt)
                   .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CartToken);

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Items)
                   .WithOne(x => x.Cart)
                   .HasForeignKey(x => x.CartId)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
