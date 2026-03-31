using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration
{
   

    public class DestinationConfiguration : BaseEntityConfigurations<Destination, Guid>
    {
        public override void Configure(EntityTypeBuilder<Destination> builder)
        {
            base.Configure(builder);


           

            builder.Property(d => d.CountryCode)
                   .HasConversion<int>();

            builder.Property(d => d.ImageUrl)
                   .HasMaxLength(500);

            builder.Property(d => d.IsActive)
                    .HasDefaultValue(true);


            builder.HasIndex(d => d.CountryCode);
            builder.HasIndex(d => d.IsActive);

          
        }
    }
}

