using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration
{

    public class FavoritesConfiguration : IEntityTypeConfiguration<Favorites>
    {
        public void Configure(EntityTypeBuilder<Favorites> builder)
        {
            builder.HasKey(f => new { f.TourId, f.UserId });

            builder.HasOne(f => f.Tour)
                   .WithMany()
                   .HasForeignKey(f => f.TourId);

            builder.HasOne(f => f.User)
                   .WithMany()
                   .HasForeignKey(f => f.UserId);

            builder.HasIndex(f => f.UserId);



            //base info :
            builder.Property(e => e.CreatedBy);

            builder.Property(e => e.CreatedDate)
                   .HasDefaultValueSql("TIMEZONE('UTC', NOW())")
                   .IsRequired();

            builder.Property(e => e.ModifiedBy);

            builder.Property(e => e.ModifiedDate)
                .HasDefaultValueSql("TIMEZONE('UTC', NOW())");
            builder.Property(e => e.IsDeleted)
                   .IsRequired()
                   .HasDefaultValue(false);
        }
    }
}
