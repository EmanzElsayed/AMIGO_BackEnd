using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{

    public class TourNotIncludedConfiguration
        : BaseEntityConfigurations<TourNotIncluded, Guid>
    {
        public override void Configure(EntityTypeBuilder<TourNotIncluded> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.NotIncluded)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.Language)
                   .HasConversion<int>();

            builder.HasIndex(x => x.TourId);

            builder.HasIndex(x => new { x.TourId, x.Language });

            builder.HasOne(x => x.Tour)
                   .WithMany(p => p.NotIncluded)
                   .HasForeignKey(x => x.TourId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
