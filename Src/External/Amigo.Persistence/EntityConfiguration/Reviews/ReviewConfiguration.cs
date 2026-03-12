using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Reviews
{
   

    public class ReviewConfiguration : BaseEntityConfigurations<Review, Guid>
    {
        public override void Configure(EntityTypeBuilder<Review> builder)
        {
            base.Configure(builder);

           
            builder.Property(r => r.Rate)
                   .HasColumnType("decimal(3,2)");

            builder.Property(r => r.Date)
                   .HasColumnType("date");

            builder.HasIndex(r => r.TourId);
            builder.HasIndex(r => r.UserId);

           

            builder.HasMany(r => r.Images)
                   .WithOne(i => i.Review)
                   .HasForeignKey(i => i.ReviewId);
        }
    }
}
