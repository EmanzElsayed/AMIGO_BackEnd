using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{

    public class PriceConfiguration : BaseEntityConfigurations<Price, Guid>
    {
        public override void Configure(EntityTypeBuilder<Price> builder)
        {
            base.Configure(builder);

           

            builder.Property(p => p.Type)
                   .HasMaxLength(400)
                   .IsRequired();

            builder.Property(p => p.Cost)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(p => p.TourId);

            builder.HasIndex(p => new { p.TourId, p.Type })
                   .IsUnique();
        }
    }
}
