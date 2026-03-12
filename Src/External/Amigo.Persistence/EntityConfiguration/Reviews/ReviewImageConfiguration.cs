using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Reviews
{
 
    public class ReviewImageConfiguration
        : BaseEntityConfigurations<ReviewImage, Guid>
    {
        public override void Configure(EntityTypeBuilder<ReviewImage> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Image)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasIndex(x => x.ReviewId);

            builder.HasOne(x => x.Review)
                   .WithMany()
                   .HasForeignKey(x => x.ReviewId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
