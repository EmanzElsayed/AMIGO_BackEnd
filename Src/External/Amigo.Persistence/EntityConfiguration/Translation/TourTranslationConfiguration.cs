using Amigo.Domain.Entities.TranslationEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Translation
{
    
    public class TourTranslationConfiguration
        : BaseEntityConfigurations<TourTranslation, Guid>
    {
        public override void Configure(EntityTypeBuilder<TourTranslation> builder)
        {
            base.Configure(builder);

            builder.Property(t => t.Title)
                   .HasMaxLength(400)
                   .IsRequired();

            builder.Property(t => t.Description)
                   .HasMaxLength(2000);

            builder.Property(t => t.Language)
                   .HasConversion<int>();

            builder.HasIndex(t => new { t.TourId, t.Language })
                   .IsUnique();

            builder.HasOne(t => t.Tour)
                   .WithMany(x => x.Translations)
                   .HasForeignKey(t => t.TourId);
        }
    }
}
