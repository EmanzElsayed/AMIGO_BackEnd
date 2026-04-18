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

            builder.Property(x => x.RetailPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Ignore(x => x.FinalPrice);

            builder.HasIndex(x => x.OrderItemId);
        }
    }
}
