using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Translation
{
    public class InclusionTranslationConfiguration : BaseEntityConfigurations<InclusionTranslation, Guid>
    {
        public override void Configure(EntityTypeBuilder<InclusionTranslation> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Text)
                  .IsRequired()
                  .HasMaxLength(400);

            builder.Property(x => x.Language)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => new { x.TourInclusionId, x.Language })
                   .IsUnique();

            builder.HasIndex(x => x.Language);

            builder.HasOne(x => x.TourInclusion)
                   .WithMany(x => x.Translations)
                   .HasForeignKey(x => x.TourInclusionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
