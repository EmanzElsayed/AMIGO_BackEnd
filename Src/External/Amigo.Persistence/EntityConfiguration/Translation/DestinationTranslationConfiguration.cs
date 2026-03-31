using Amigo.Domain.Entities.TranslationEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Amigo.Persistence.EntityConfiguration.Translation
{
   

    public class DestinationTranslationConfiguration
        : BaseEntityConfigurations<DestinationTranslation, Guid>
    {
        public override void Configure(EntityTypeBuilder<DestinationTranslation> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(x => x.Language)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => new { x.DestinationId, x.Language })
                   .IsUnique();

            builder.HasIndex(x => x.Language);

            builder.HasOne(x => x.Destination)
                   .WithMany(x => x.Translations)
                   .HasForeignKey(x => x.DestinationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
