using Amigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amigo.Persistence.EntityConfiguration;

public class InterestedTourConfiguration : IEntityTypeConfiguration<InterestedTour>
{
    public void Configure(EntityTypeBuilder<InterestedTour> builder)
    {
        builder.HasKey(x => new { x.TourId, x.InterestedInTourId });

        builder
            .HasOne(x => x.Tour)
            .WithMany(t => t.RelatedTours)
            .HasForeignKey(x => x.TourId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.InterestedInTour)
            .WithMany(t => t.RelatedFromTours)
            .HasForeignKey(x => x.InterestedInTourId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
