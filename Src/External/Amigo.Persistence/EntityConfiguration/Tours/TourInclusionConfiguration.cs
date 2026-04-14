using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Tours
{
    public class TourInclusionConfiguration 
        : BaseEntityConfigurations<TourInclusion, Guid>
    {
        public override void Configure(EntityTypeBuilder<TourInclusion> builder)
        {
            base.Configure(builder);

            builder.Property(t => t.IsIncluded)
                 .IsRequired()
                 .HasDefaultValue(false);


            builder.HasIndex(p => p.TourId);

            builder.HasOne(p => p.Tour)
                           .WithMany(x => x.TourInclusions)
                           .HasForeignKey(p => p.TourId)
                           .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
