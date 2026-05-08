using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Orders
{
    public class OrderedPriceConfiguration : BaseEntityConfigurations<OrderedPrice, Guid>
    {
        public override void Configure(EntityTypeBuilder<OrderedPrice> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Type)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.BaseRetailPrice)
                   .HasColumnType("decimal(18,4)");


            builder.Property(x => x.ConvertedRetailPrice)
                  .HasColumnType("decimal(18,4)");

            builder.Property(x => x.ExchangeRate)
                .HasColumnType("decimal(18,4)");

            builder.Ignore(x => x.FinalPrice);

            builder.HasIndex(x => x.OrderItemId);
        }
    }
}
