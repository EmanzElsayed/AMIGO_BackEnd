using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration
{
    public class CurrencyConfiguration : BaseEntityConfigurations<Currency, Guid>
    {
        public override void Configure(EntityTypeBuilder<Currency> builder)
        {
            base.Configure(builder);
            builder.Property(d => d.CurrencyCode)
                   .HasConversion<int>();

            builder.HasIndex(d => d.CurrencyCode);
        }
    }
}
