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

           

            builder.Property(t => t.UserType)
                  .HasConversion<int>();


            builder.Property(t => t.Discount)
                  .HasColumnType("decimal(18,2)");


            builder.Property(p => p.Cost)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(p => p.TourId);

            builder.HasIndex(p => new { p.TourId, p.UserType });



            builder.HasOne(p => p.Tour)
                .WithMany(x => x.Prices)
                .HasForeignKey(p => p.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
