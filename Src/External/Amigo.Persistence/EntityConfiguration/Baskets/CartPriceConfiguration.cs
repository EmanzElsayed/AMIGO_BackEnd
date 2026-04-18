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

            builder.Property(x => x.RetailPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Ignore(x => x.FinalPrice);

            builder.HasIndex(x => x.CartItemId);
        }
    }
}
