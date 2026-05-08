using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Baskets
{
    public class CartPriceConfiguration : BaseEntityConfigurations<CartPrice, Guid>
    {
        public override void Configure(EntityTypeBuilder<CartPrice> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Type)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.BaseRetailPrice)
                   .HasColumnType("decimal(18,4)");

            builder.Property(x => x.ConvertedRetailPrice)
                  .HasColumnType("decimal(18,4)");

            builder.Property(x => x.ExchangeRate)
                .HasColumnType("decimal(18,4)");

            builder.Ignore(x => x.FinalPrice);

            builder.HasIndex(x => x.CartItemId);
        }
    }
}
