using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration;

public class CurrencyRateConfiguration:BaseEntityConfigurations<CurrencyRate,Guid>
{
    public override void Configure(EntityTypeBuilder<CurrencyRate> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.BaseCurrency)
          .HasConversion<int>();

        builder.Property(x => x.TargetCurrency)
            .HasConversion<int>();

        builder.Property(x => x.Rate)
            .HasPrecision(18, 8);

        builder.Property(x => x.Provider)
            .HasMaxLength(100);

        builder.HasIndex(x => new
        {
            x.BaseCurrency,
            x.TargetCurrency
        }).IsUnique();
    }
}