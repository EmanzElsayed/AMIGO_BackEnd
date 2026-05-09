using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration
{
    public class CountryInfoConfiguration : BaseEntityConfigurations<CountryInfo, Guid>
    {
        public override void Configure(EntityTypeBuilder<CountryInfo> builder)
        {
            base.Configure(builder);

            builder.Property(d => d.CountryCode)
                   .HasConversion<int>();

            builder.HasIndex(d => d.CountryCode);

            builder.HasMany(c => c.Destinations)
                .WithOne(d => d.CountryInfo)
                .HasForeignKey(d => d.CountryInfoId)
                 .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
