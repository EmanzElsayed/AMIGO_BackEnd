using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{

    public class TourImageConfiguration
        : BaseEntityConfigurations<TourImage, Guid>
    {
        public override void Configure(EntityTypeBuilder<TourImage> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.ImagePublicId)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasIndex(x => x.TourId);

            builder.HasOne(x => x.Tour)
                   .WithMany(p => p.Images)
                   .HasForeignKey(x => x.TourId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
