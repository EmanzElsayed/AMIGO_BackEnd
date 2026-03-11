using Amigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amigo.Persistence.EntityConfiguration;

public class TourCategoryConfiguration : IEntityTypeConfiguration<TourCategory>
{
    public void Configure(EntityTypeBuilder<TourCategory> builder)
    {
        builder.HasKey(x => new { x.TourId, x.CategoryId });

        builder
            .HasOne(x => x.Tour)
            .WithMany(t => t.Categories)
            .HasForeignKey(x => x.TourId);

        builder
            .HasOne(x => x.Category)
            .WithMany(c => c.Tours)
            .HasForeignKey(x => x.CategoryId);
    }
}
